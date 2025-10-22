// editCourse.js - version_JS: 2.3.2 (patched)
// Основные правки: улучшенная нормализация, paste/insert обработчики, удаление комментариев,
// сохранение единственного корневого DIV, предотвращение вложенных <p>.

console.warn('version_JS: 2.3.2');

// ---------- Utilities ----------
function debounce(fn, ms) { let t; return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), ms); }; }
function getAntiForgeryToken() { const el = document.querySelector('input[name="__RequestVerificationToken"]'); return el ? el.value : ''; }
async function postUrlEncoded(url, obj) {
    const token = getAntiForgeryToken();
    const params = new URLSearchParams();
    for (const k in obj) if (obj[k] !== undefined && obj[k] !== null) params.append(k, obj[k]);
    const resp = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token,
            'X-Requested-With': 'XMLHttpRequest',
            'Accept': 'application/json'
        },
        body: params.toString()
    });
    try { return await resp.json(); } catch { return { success: false, message: 'Invalid server response' }; }
}

async function getJson(url) {
    try {
        const r = await fetch(url);
        return await r.json();
    } catch (err) {
        console.error('getJson failed for', url, err);
        return { success: false, message: 'Invalid server response' };
    }
}

function decodeHtmlEntitiesIfNeeded(str) {
    if (!str) return '';
    if (str.indexOf('&lt;') === -1 && str.indexOf('&gt;') === -1 && str.indexOf('&amp;') === -1) return str;
    const parser = new DOMParser();
    const doc = parser.parseFromString(str, 'text/html');
    return doc.documentElement.textContent || '';
}

// Pretty-print HTML for code view (simple formatter)
function prettyPrintHtml(html) {
    if (!html) return '';
    let s = html.replace(/>\s+</g, '><');
    s = s.replace(/></g, '>' + '\n' + '<');
    const lines = s.split('\n');
    let indent = 0;
    const out = [];
    lines.forEach(line => {
        const trimmed = line.trim();
        if (!trimmed) return;
        if (/^<\/.+>/.test(trimmed)) {
            indent = Math.max(indent - 1, 0);
        }
        out.push('  '.repeat(indent) + trimmed);
        if (/^<[^\/!][^>]*[^\/]$/.test(trimmed) && !/^<.*<.*>.*<.*>$/.test(trimmed) && !/^<[^>]+>.*<\/[^>]+>$/.test(trimmed)) {
            if (!/^<.*<\/.*>$/.test(trimmed) && !/\/>$/.test(trimmed)) indent++;
        }
    });
    return out.join('\n');
}

// Find ancestor helper
function findAncestor(node, tag) {
    while (node && node.nodeType === 1) {
        if (node.tagName && node.tagName.toLowerCase() === tag) return node;
        node = node.parentElement;
    }
    return null;
}

// Escape HTML for plain-text -> html insertion
function escapeHtml(s) {
    if (s == null) return '';
    return String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

// ---------- Helpers for block detection ----------
function isBlockTag(node) {
    if (!node || node.nodeType !== 1) return false;
    const t = node.tagName.toLowerCase();
    return /^(p|div|h[1-6]|ul|ol|li|table|thead|tbody|tr|td|th|pre|blockquote|section|article|aside|header|footer)$/i.test(t);
}
function isInlineTagName(tagName) {
    if (!tagName) return false;
    return /^(span|a|strong|b|i|em|img|code|small|sub|sup|u|mark|time|br)$/i.test(tagName);
}

// ---------- Editor sanitization & normalization ----------

// Remove font-size/line-height/font-family inline styles from element and descendants
function stripFontAttrs(el) {
    if (!el || el.nodeType !== 1) return;
    try {
        if (el.style) {
            el.style.fontSize = '';
            el.style.lineHeight = '';
            el.style.fontFamily = '';
            if (el.getAttribute('style') && el.getAttribute('style').trim() === '') el.removeAttribute('style');
        }
    } catch (e) { }
    Array.from(el.querySelectorAll ? el.querySelectorAll('[style]') : []).forEach(child => {
        try {
            child.style.fontSize = '';
            child.style.lineHeight = '';
            child.style.fontFamily = '';
            if (child.getAttribute('style') && child.getAttribute('style').trim() === '') child.removeAttribute('style');
        } catch (e) { }
    });
}

// remove comments recursively from node
function removeCommentsFromNode(node) {
    if (!node) return;
    const walker = document.createTreeWalker(node, NodeFilter.SHOW_COMMENT, null, false);
    const toRemove = [];
    let n;
    while ((n = walker.nextNode())) toRemove.push(n);
    toRemove.forEach(c => c.parentNode && c.parentNode.removeChild(c));
}

// Normalize content inside visual editor:
// - preserve single top-level wrapper DIV (if present) — do not convert it to <p>
// - wrap top-level text nodes into <p>
// - convert top-level <div> -> <p> only when that div is not a meaningful wrapper
// - strip font-related inline styles across nodes
// - avoid nested <p> i.e. do not create <p> inside existing <p>
function normalizeEditorContent(editorEl) {
    if (!editorEl) return;

    // remove any comment nodes inside editor
    removeCommentsFromNode(editorEl);

    // if editor has exactly one top-level element and it's a DIV, treat it as the original wrapper:
    const topChildren = Array.from(editorEl.childNodes);
    if (topChildren.length === 1 && topChildren[0].nodeType === Node.ELEMENT_NODE && topChildren[0].tagName.toLowerCase() === 'div') {
        // preserve wrapper div but strip font styles inside it and normalize its children
        const wrapper = topChildren[0];
        stripFontAttrs(wrapper);
        removeCommentsFromNode(wrapper);

        // normalize children of wrapper (wrap stray text nodes into <p>, but don't convert existing block tags)
        const innerChildren = Array.from(wrapper.childNodes);
        for (let node of innerChildren) {
            if (node.nodeType === Node.COMMENT_NODE) {
                wrapper.removeChild(node);
                continue;
            }
            if (node.nodeType === Node.TEXT_NODE) {
                if (node.textContent.trim()) {
                    // if the text node is already inside a block (it is direct child of wrapper), create p
                    const p = document.createElement('p');
                    p.textContent = node.textContent;
                    wrapper.replaceChild(p, node);
                } else {
                    wrapper.removeChild(node);
                }
            } else if (node.nodeType === Node.ELEMENT_NODE) {
                // if it's a div inside wrapper and contains only inline content, convert to p
                const t = node.tagName.toLowerCase();
                if (t === 'div') {
                    // if div contains any block child, keep as-is (but try to unwrap single block child)
                    const hasBlockChild = Array.from(node.childNodes).some(ch => ch.nodeType === 1 && isBlockTag(ch));
                    if (!hasBlockChild) {
                        const p = document.createElement('p');
                        p.innerHTML = node.innerHTML;
                        Array.from(node.attributes).forEach(attr => { if (attr.name !== 'style') p.setAttribute(attr.name, attr.value); });
                        stripFontAttrs(p);
                        wrapper.replaceChild(p, node);
                    } else if (node.childElementCount === 1 && isBlockTag(node.firstElementChild)) {
                        // unwrap: move single block child up preserving attributes
                        const only = node.firstElementChild;
                        Array.from(node.attributes).forEach(attr => { if (attr.name !== 'style' && !only.hasAttribute(attr.name)) only.setAttribute(attr.name, attr.value); });
                        stripFontAttrs(only);
                        wrapper.replaceChild(only, node);
                    } else {
                        stripFontAttrs(node);
                    }
                } else {
                    stripFontAttrs(node);
                }
            } else {
                try { wrapper.removeChild(node); } catch (e) { }
            }
        }
        return;
    }

    // Otherwise proceed with normalization across top-level nodes
    // Remove comments
    // (already removed on editorEl)
    const children = Array.from(editorEl.childNodes);
    for (let node of children) {
        if (node.nodeType === Node.COMMENT_NODE) {
            editorEl.removeChild(node);
            continue;
        }
        if (node.nodeType === Node.TEXT_NODE) {
            if (node.textContent.trim()) {
                // only create <p> if parent is editor (i.e., not inside some block)
                const p = document.createElement('p');
                p.textContent = node.textContent;
                editorEl.replaceChild(p, node);
            } else {
                editorEl.removeChild(node);
            }
        } else if (node.nodeType === Node.ELEMENT_NODE) {
            const tag = node.tagName.toLowerCase();
            if (tag === 'div') {
                // If div contains only inline content -> convert to <p>
                const hasBlockChild = Array.from(node.childNodes).some(ch => ch.nodeType === 1 && isBlockTag(ch));
                const onlyChild = node.childElementCount === 1 ? node.firstElementChild : null;
                if (!hasBlockChild) {
                    // But avoid creating <p> if the div already starts with <p> inside -> unwrap children
                    if (/^\s*<(p|h[1-6]|ul|ol|table|pre|blockquote)\b/i.test(node.innerHTML.trim())) {
                        const frag = document.createDocumentFragment();
                        Array.from(node.childNodes).forEach(cn => frag.appendChild(cn.cloneNode(true)));
                        editorEl.replaceChild(frag, node);
                    } else {
                        const p = document.createElement('p');
                        p.innerHTML = node.innerHTML;
                        Array.from(node.attributes).forEach(attr => { if (attr.name !== 'style') p.setAttribute(attr.name, attr.value); });
                        stripFontAttrs(p);
                        editorEl.replaceChild(p, node);
                    }
                } else if (onlyChild && isBlockTag(onlyChild)) {
                    // unwrap single block child
                    Array.from(node.attributes).forEach(attr => { if (attr.name !== 'style' && !onlyChild.hasAttribute(attr.name)) onlyChild.setAttribute(attr.name, attr.value); });
                    stripFontAttrs(onlyChild);
                    editorEl.replaceChild(onlyChild, node);
                } else {
                    stripFontAttrs(node);
                }
            } else {
                stripFontAttrs(node);
            }
        } else {
            try { editorEl.removeChild(node); } catch (e) { }
        }
    }

    // final pass: remove <p> that contains only another <p> (avoid nested)
    Array.from(editorEl.querySelectorAll('p')).forEach(p => {
        if (p.parentElement && p.parentElement.tagName && p.parentElement.tagName.toLowerCase() === 'p') {
            // move children of p into parent and remove nested p
            const parent = p.parentElement;
            while (p.firstChild) parent.insertBefore(p.firstChild, p);
            parent.removeChild(p);
        }
    });
}

// ---------- Insert sanitized HTML at current cursor in visual editor ----------
function insertHtmlAtCursor(html) {
    const editor = document.getElementById('modalLessonVisual');
    if (!editor) return;
    editor.focus();

    // normalize incoming html string: remove comments and zero-width characters
    html = String(html || '');
    html = html.replace(/<!--[\s\S]*?-->/g, '');
    html = html.replace(/[\u200B-\u200D\uFEFF]/g, ''); // zero-width & BOM

    if (window.getSelection) {
        const sel = window.getSelection();
        if (sel.getRangeAt && sel.rangeCount) {
            const range = sel.getRangeAt(0);
            // build a container and sanitize basic inline font styles
            const tmp = document.createElement('div');
            tmp.innerHTML = html;

            removeCommentsFromNode(tmp);
            Array.from(tmp.querySelectorAll('[style]')).forEach(e => {
                try {
                    e.style.fontSize = '';
                    e.style.lineHeight = '';
                    e.style.fontFamily = '';
                    if (!e.getAttribute('style') || e.getAttribute('style').trim() === '') e.removeAttribute('style');
                } catch (ex) { }
            });

            // If tmp has single DIV wrapper and that DIV is likely the original wrapper from copy,
            // prefer to insert its children (so we don't create nested wrapper)
            let nodesToInsert = Array.from(tmp.childNodes);
            if (nodesToInsert.length === 1 && nodesToInsert[0].nodeType === 1 && nodesToInsert[0].tagName.toLowerCase() === 'div') {
                const wrapper = nodesToInsert[0];
                // if wrapper contains only a single block child that is a <p> or similar - insert that child instead
                if (wrapper.childElementCount === 1 && isBlockTag(wrapper.firstElementChild)) {
                    nodesToInsert = [wrapper.firstElementChild.cloneNode(true)];
                } else {
                    nodesToInsert = Array.from(wrapper.childNodes).map(n => n.cloneNode(true));
                }
            } else {
                nodesToInsert = nodesToInsert.map(n => n.cloneNode(true));
            }

            // Build fragment but avoid inserting <p> inside an existing <p>
            const frag = document.createDocumentFragment();
            const anchorP = findAncestor(range.startContainer, 'p');

            nodesToInsert.forEach(node => {
                if (node.nodeType === Node.TEXT_NODE) {
                    if (node.textContent.trim()) {
                        const p = document.createElement('p');
                        p.textContent = node.textContent;
                        frag.appendChild(p);
                    }
                } else if (node.nodeType === Node.ELEMENT_NODE) {
                    const t = node.tagName.toLowerCase();
                    // if we're inside a <p> and inserting a block <p>, unwrap the inserted <p> into sibling nodes instead
                    if (anchorP && t === 'p') {
                        // move children of node as nodes
                        Array.from(node.childNodes).forEach(ch => frag.appendChild(ch.cloneNode(true)));
                    } else {
                        frag.appendChild(node);
                    }
                }
            });

            // insert fragment
            range.deleteContents();
            range.insertNode(frag);

            // restore caret after inserted content
            sel.removeAllRanges();
            const newRange = document.createRange();
            // find last inserted node in editor
            let last = null;
            if (frag && frag.lastChild) last = frag.lastChild;
            else last = editor.childNodes[editor.childNodes.length - 1] || editor;
            try {
                if (last.nodeType === Node.TEXT_NODE) newRange.setStartAfter(last.parentNode);
                else newRange.setStartAfter(last);
            } catch (e) {
                try { newRange.setStart(editor, editor.childNodes.length); } catch (ex) { newRange.setStart(editor, 0); }
            }
            newRange.collapse(true);
            sel.addRange(newRange);
        }
    } else {
        editor.innerHTML += html;
    }

    // final normalization & sync
    normalizeEditorContent(editor);
    syncFromVisual();
    pushHistory();
}

// ---------- Insert / Replace <pre data-editor="..."> helper (keeps original behaviour) ----------
function insertOrReplacePre(type, lang) {
    const editor = document.getElementById('lesson-body') || document.getElementById('modalLessonVisual');
    if (!editor) return;
    editor.focus();
    const sel = window.getSelection();
    let ancestor = sel && sel.anchorNode ? findAncestor(sel.anchorNode, 'pre') : null;
    if (ancestor) {
        ancestor.setAttribute('data-editor', type === 'code' ? 'code' : type);
        if (type === 'code' && lang) ancestor.setAttribute('data-ln', lang);
        syncFromVisual();
        pushHistory();
        return;
    }
    const content = (type === 'code') ? `<pre data-editor="code" data-ln="${lang}"><code>// code sample</code></pre>` : `<pre data-editor="${type}">Output</pre>`;
    insertHtmlAtCursor(content);
}

// ---------- Code dropdown binding (improved) ----------
(function bindCodeDropdown() {
    const cd = document.getElementById('codeDropdown');
    if (!cd) return;
    try { if (cd._handler) cd.removeEventListener('click', cd._handler); } catch (e) { }
    const handler = function (e) {
        const btn = e.target.closest('[data-code-type]'); if (!btn) return;
        const type = btn.dataset.codeType;
        if (type === 'code') {
            const lang = prompt('Enter language (e.g., js, python):') || 'js';
            insertOrReplacePre('code', lang);
        } else {
            insertOrReplacePre(type);
        }
        hideAllDropdowns();
    };
    cd.addEventListener('click', handler);
    cd._handler = handler;
})();

// ---------- History (undo/redo) ----------
let editHistory = [];
let historyIndex = -1;
function pushHistory() {
    const el = document.getElementById('modalLessonVisual');
    if (!el) return;
    const v = el.innerHTML;
    if (historyIndex < editHistory.length - 1) editHistory = editHistory.slice(0, historyIndex + 1);
    editHistory.push(v);
    historyIndex = editHistory.length - 1;
    if (editHistory.length > 200) { editHistory.shift(); historyIndex--; }
}
function undo() { if (historyIndex > 0) { historyIndex--; const v = editHistory[historyIndex]; document.getElementById('modalLessonVisual').innerHTML = v; syncFromVisual(); } }
function redo() { if (historyIndex < editHistory.length - 1) { historyIndex++; const v = editHistory[historyIndex]; document.getElementById('modalLessonVisual').innerHTML = v; syncFromVisual(); } }

// ---------- Sync visual <-> hidden <-> code view ----------
function syncFromVisual() {
    const visual = document.getElementById('modalLessonVisual');
    const hidden = document.getElementById('modalLessonHtml');
    const code = document.getElementById('lessonCodeView');
    if (!visual || !hidden || !code) return;
    // canonical source is visual.innerHTML
    const html = visual.innerHTML;
    hidden.value = html;
    code.textContent = prettyPrintHtml(html);
}

// ---------- Dropdown helpers ----------
function showDropdown(dropdownId, btn) {
    hideAllDropdowns();
    const dropdown = document.getElementById(dropdownId); if (!dropdown || !btn) return;
    const rect = btn.getBoundingClientRect();
    dropdown.style.position = 'fixed';
    dropdown.style.top = (rect.bottom + 8) + 'px';
    dropdown.style.left = rect.left + 'px';
    dropdown.classList.add('active');
    setTimeout(() => {
        const dropRect = dropdown.getBoundingClientRect();
        if (dropRect.bottom > window.innerHeight) dropdown.style.top = (rect.top - dropRect.height - 8) + 'px';
        if (dropRect.right > window.innerWidth) dropdown.style.left = (window.innerWidth - dropRect.width - 10) + 'px';
    }, 10);
}
function hideAllDropdowns() { document.querySelectorAll('.dropdown-menu').forEach(d => d.classList.remove('active')); }

// ---------- Toolbar handling (integrated) ----------
const toolbar = document.querySelector('.editor-toolbar');
if (toolbar) {
    toolbar.addEventListener('click', function (e) {
        const btn = e.target.closest('.toolbar-btn'); if (!btn) return;
        const format = btn.dataset.format;
        switch (format) {
            case 'undo': undo(); break;
            case 'redo': redo(); break;
            case 'bold': document.execCommand('bold'); syncFromVisual(); pushHistory(); break;
            case 'italic': document.execCommand('italic'); syncFromVisual(); pushHistory(); break;
            case 'strong': document.execCommand('bold'); syncFromVisual(); pushHistory(); break;
            case 'h3': document.execCommand('formatBlock', false, 'h3'); syncFromVisual(); pushHistory(); break;
            case 'h4': document.execCommand('formatBlock', false, 'h4'); syncFromVisual(); pushHistory(); break;
            case 'list': showDropdown('listDropdown', btn); break;
            case 'code': showDropdown('codeDropdown', btn); break;
            case 'image': {
                const url = prompt('Enter image URL:'); if (url) insertHtmlAtCursor(`<img src="${url}" alt="Image" />`);
                break;
            }
            case 'table': showDropdown('tableDropdown', btn); break;
            case 'clear':
                clearStylesForSelection('p');
                break;
        }
    });
}

// ---------- List dropdown ----------
const listDropdown = document.getElementById('listDropdown');
if (listDropdown) {
    listDropdown.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-list-type]'); if (!btn) return;
        const type = btn.dataset.listType;
        if (type === 'ul') insertHtmlAtCursor('<ul><li>List item</li></ul>'); else insertHtmlAtCursor('<ol><li>List item</li></ol>');
        hideAllDropdowns();
    });
}

// ---------- Code dropdown (alternate handler kept for backwards compat if needed) ----------
const codeDropdown = document.getElementById('codeDropdown');
if (codeDropdown) {
    codeDropdown.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-code-type]'); if (!btn) return;
        const type = btn.dataset.codeType;
        if (type === 'code') {
            const lang = prompt('Enter language (e.g., js, python):') || 'js';
            insertHtmlAtCursor(`<pre data-editor="code" data-ln="${lang}"><code>// code sample</code></pre>`);
        } else {
            insertHtmlAtCursor(`<pre data-editor="${type}">Output</pre>`);
        }
        hideAllDropdowns();
    });
}

// ---------- Table picker ----------
const tablePicker = document.getElementById('tablePicker');
if (tablePicker) {
    // create 10x10 picker
    tablePicker.innerHTML = '';
    for (let i = 0; i < 100; i++) {
        const cell = document.createElement('div'); cell.className = 'table-cell-picker'; cell.dataset.row = Math.floor(i / 10) + 1; cell.dataset.col = (i % 10) + 1; tablePicker.appendChild(cell);
    }
    tablePicker.addEventListener('mouseover', function (e) {
        if (!e.target.classList.contains('table-cell-picker')) return;
        const row = parseInt(e.target.dataset.row); const col = parseInt(e.target.dataset.col);
        document.querySelectorAll('.table-cell-picker').forEach(c => {
            const r = parseInt(c.dataset.row); const cl = parseInt(c.dataset.col);
            if (r <= row && cl <= col) c.classList.add('selected'); else c.classList.remove('selected');
        });
        const sz = document.getElementById('tableSize'); if (sz) sz.textContent = `${row}x${col}`;
    });
    tablePicker.addEventListener('click', function (e) {
        if (!e.target.classList.contains('table-cell-picker')) return;
        const row = parseInt(e.target.dataset.row); const col = parseInt(e.target.dataset.col);
        let table = '<table>';
        for (let r = 0; r < row; r++) {
            table += '<tr>';
            for (let c = 0; c < col; c++) table += '<td>Cell</td>';
            table += '</tr>';
        }
        table += '</table>';
        insertHtmlAtCursor(table);
        hideAllDropdowns();
    });
}

// ---------- Paste handling: sanitize pasted HTML/text ----------
(function bindPasteHandler() {
    const editor = document.getElementById('modalLessonVisual');
    if (!editor) return;
    editor.addEventListener('paste', function (e) {
        e.preventDefault();
        const clipboard = (e.clipboardData || window.clipboardData);
        let html = clipboard.getData('text/html');
        const text = clipboard.getData('text/plain');

        if (!html && text) {
            html = text.split('\n').map(line => `<p>${escapeHtml(line)}</p>`).join('');
        }
        if (!html) return;

        // strip HTML comments (e.g., <!--StartFragment-->) and zero-width chars
        html = html.replace(/<!--[\s\S]*?-->/g, '');
        html = html.replace(/[\u200B-\u200D\uFEFF]/g, '');

        const container = document.createElement('div');
        container.innerHTML = html;

        removeCommentsFromNode(container);

        // remove font inline styles and attributes
        Array.from(container.querySelectorAll('[style]')).forEach(el => {
            try {
                el.style.fontSize = '';
                el.style.lineHeight = '';
                el.style.fontFamily = '';
                if (!el.getAttribute('style') || el.getAttribute('style').trim() === '') el.removeAttribute('style');
            } catch (ex) { }
        });

        // convert DIV -> P safely (avoid double-p or nested p)
        Array.from(container.querySelectorAll('div')).forEach(d => {
            const onlyChild = d.childElementCount === 1 ? d.firstElementChild : null;
            if (onlyChild && /^(p|h[1-6]|ul|ol|table|pre|blockquote)$/i.test(onlyChild.tagName)) {
                // move attributes if needed
                Array.from(d.attributes).forEach(attr => { if (attr.name !== 'style' && !onlyChild.hasAttribute(attr.name)) onlyChild.setAttribute(attr.name, attr.value); });
                d.parentNode.replaceChild(onlyChild, d);
            } else {
                if (/^\s*<(p|h[1-6]|ul|ol|table|pre|blockquote)\b/i.test(d.innerHTML.trim())) {
                    // move children out
                    const frag = document.createDocumentFragment();
                    Array.from(d.childNodes).forEach(cn => frag.appendChild(cn.cloneNode(true)));
                    d.parentNode.replaceChild(frag, d);
                } else {
                    // only convert if div doesn't contain block children
                    const hasBlockChild = Array.from(d.childNodes).some(ch => ch.nodeType === 1 && isBlockTag(ch));
                    if (!hasBlockChild) {
                        const p = document.createElement('p');
                        p.innerHTML = d.innerHTML;
                        Array.from(d.attributes).forEach(attr => { if (attr.name !== 'style') p.setAttribute(attr.name, attr.value); });
                        d.parentNode.replaceChild(p, d);
                    } else {
                        // try to unwrap single block child
                        if (onlyChild && isBlockTag(onlyChild)) {
                            Array.from(d.attributes).forEach(attr => { if (attr.name !== 'style' && !onlyChild.hasAttribute(attr.name)) onlyChild.setAttribute(attr.name, attr.value); });
                            d.parentNode.replaceChild(onlyChild, d);
                        } else {
                            // keep as-is but strip font attrs
                            stripFontAttrs(d);
                        }
                    }
                }
            }
        });

        // wrap stray text nodes into <p>
        const frag = document.createDocumentFragment();
        Array.from(container.childNodes).forEach(n => {
            if (n.nodeType === Node.TEXT_NODE) {
                if (n.textContent.trim()) {
                    const p = document.createElement('p'); p.textContent = n.textContent; frag.appendChild(p);
                }
            } else {
                frag.appendChild(n.cloneNode(true));
            }
        });

        const tmp = document.createElement('div'); tmp.appendChild(frag);
        insertHtmlAtCursor(tmp.innerHTML);
    });
})();

// ---------- Enter behavior: prefer <p> paragraphs ----------
(function bindEnterBehavior() {
    const editor = document.getElementById('modalLessonVisual');
    if (!editor) return;
    editor.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            try { document.execCommand('insertParagraph'); } catch (ex) {
                // fallback: insert <p><br></p>
                insertHtmlAtCursor('<p><br></p>');
            }
            setTimeout(() => { normalizeEditorContent(editor); syncFromVisual(); pushHistory(); }, 0);
        }
    });
})();

// ---------- Clear styles for selection and enforce tag (default 'p') ----------
function clearStylesForSelection(tagName = 'p') {
    const editor = document.getElementById('modalLessonVisual');
    if (!editor) return;
    const sel = window.getSelection();
    if (!sel || !sel.rangeCount) return;
    const range = sel.getRangeAt(0);

    if (range.collapsed) {
        // find nearest block
        const anc = findAncestor(range.startContainer, 'p') ||
            findAncestor(range.startContainer, 'div') ||
            findAncestor(range.startContainer, 'h3') ||
            findAncestor(range.startContainer, 'h4') ||
            findAncestor(range.startContainer, 'li') ||
            null;
        if (!anc) {
            const p = document.createElement(tagName);
            p.innerHTML = '<br>';
            range.insertNode(p);
            sel.removeAllRanges();
            const r = document.createRange(); r.setStart(p, 0); r.collapse(true); sel.addRange(r);
            normalizeEditorContent(editor); syncFromVisual(); pushHistory();
            return;
        }
        const newEl = document.createElement(tagName);
        newEl.innerHTML = anc.innerHTML;
        Array.from(newEl.querySelectorAll('[style]')).forEach(e => e.removeAttribute('style'));
        if (newEl.hasAttribute('style')) newEl.removeAttribute('style');
        anc.parentNode.replaceChild(newEl, anc);
        normalizeEditorContent(editor); syncFromVisual(); pushHistory();
        return;
    }

    // Non-collapsed: extract content, sanitize, reinsert
    const frag = range.extractContents();
    // remove inline styles
    Array.from(frag.querySelectorAll ? frag.querySelectorAll('[style]') : []).forEach(e => e.removeAttribute('style'));
    Array.from(frag.querySelectorAll ? frag.querySelectorAll('div') : []).forEach(d => {
        const p = document.createElement(tagName);
        p.innerHTML = d.innerHTML;
        Array.from(d.attributes).forEach(attr => { if (attr.name !== 'style') p.setAttribute(attr.name, attr.value); });
        d.parentNode.replaceChild(p, d);
    });

    // wrap stray text nodes
    const newFrag = document.createDocumentFragment();
    Array.from(frag.childNodes).forEach(node => {
        if (node.nodeType === Node.TEXT_NODE) {
            if (node.textContent.trim()) {
                const p = document.createElement(tagName); p.textContent = node.textContent; newFrag.appendChild(p);
            }
        } else {
            newFrag.appendChild(node);
        }
    });

    range.insertNode(newFrag);
    sel.removeAllRanges();
    const newRange = document.createRange();
    newRange.setStartAfter(newFrag.lastChild || newFrag);
    newRange.collapse(true);
    sel.addRange(newRange);
    normalizeEditorContent(editor); syncFromVisual(); pushHistory();
}

// ---------- Visual editor input handling (sync + normalize + history) ----------
const visualEl = document.getElementById('modalLessonVisual');
if (visualEl) {
    visualEl.addEventListener('input', debounce(function () {
        normalizeEditorContent(visualEl);
        syncFromVisual();
        pushHistory();
    }, 150));
    visualEl.addEventListener('keydown', function (e) {
        if (e.ctrlKey && (e.key === 'z' || e.key === 'Z')) { e.preventDefault(); undo(); } else if (e.ctrlKey && (e.key === 'y' || e.key === 'Y')) { e.preventDefault(); redo(); }
    });
}

// ---------- When opening lesson (fill data) ----------
function openLessonModalWithData(lesson) {
    document.getElementById('modalLessonId').value = lesson.id || '';
    document.getElementById('modalLessonModuleId').value = lesson.moduleId || '';
    document.getElementById('modalLessonTitle').value = lesson.title || '';
    document.getElementById('modalLessonSlug').value = lesson.slug || '';
    document.getElementById('modalLessonPosition').value = lesson.position || 0;
    document.getElementById('modalLessonPublished').checked = !!lesson.isPublished;
    document.getElementById('modalLessonFree').checked = !!lesson.isFreePreview;

    const body = decodeHtmlEntitiesIfNeeded(lesson.body || '');
    const visual = document.getElementById('modalLessonVisual');
    const hidden = document.getElementById('modalLessonHtml');
    if (visual) {
        // if body has single top-level wrapper (div) — keep it intact
        let toInsert = body || '';
        try {
            // remove surrounding BOM/zero-width just in case
            toInsert = toInsert.replace(/[\u200B-\u200D\uFEFF]/g, '');
        } catch (e) { }
        visual.innerHTML = toInsert;
    }
    if (hidden) hidden.value = body;
    normalizeEditorContent(visual);
    syncFromVisual();
    // reset history
    editHistory = [body || '']; historyIndex = 0;
    showModal('lessonModalOverlay');
}

// ---------- Global delegated click handlers ----------
document.addEventListener('click', function (e) {
    const closeTarget = e.target.getAttribute && e.target.getAttribute('data-close'); if (closeTarget) hideModal(closeTarget);
});
document.querySelectorAll('[data-modal-overlay]').forEach(ov => { ov.addEventListener('click', function (e) { if (e.target === ov) ov.setAttribute('hidden', ''); }); });

function showModal(id) { const o = document.getElementById(id); if (!o) return; o.removeAttribute('hidden'); document.body.style.overflow = 'hidden'; }
function hideModal(id) { const o = document.getElementById(id); if (!o) return; o.setAttribute('hidden', ''); hideAllDropdowns(); document.body.style.overflow = ''; }

// ---------- Save lesson: sync hidden textarea first ----------
const btnSaveLesson = document.getElementById('btnSaveLesson');
if (btnSaveLesson) btnSaveLesson.addEventListener('click', async function () {
    const lessonId = document.getElementById('modalLessonId').value;
    const moduleId = document.getElementById('modalLessonModuleId').value;
    const title = document.getElementById('modalLessonTitle').value;
    const slug = document.getElementById('modalLessonSlug').value;
    const position = document.getElementById('modalLessonPosition').value || 0;
    const isPublished = document.getElementById('modalLessonPublished').checked;
    const isFree = document.getElementById('modalLessonFree').checked;
    // ensure hidden textarea is synced
    normalizeEditorContent(document.getElementById('modalLessonVisual'));
    syncFromVisual();
    const body = document.getElementById('modalLessonHtml').value || '';
    if (!lessonId) { alert('Lesson id missing'); return; }
    const data = { ModuleId: moduleId, LessonId: lessonId, Title: title, Slug: slug, Body: body, Position: position, IsPublished: isPublished, IsFreePreview: isFree };
    const res = await postUrlEncoded('/AdminCourse/UpdateLesson', data);
    if (res && res.success) { hideModal('lessonModalOverlay'); await loadModules(); } else alert(res && res.message ? res.message : 'Unable to save lesson');
});

// ---------- renderModules / loadModules (unchanged logic, kept here) ----------
function renderModules(modules) {
    const container = document.getElementById('modulesContainer'); if (!container) return; container.innerHTML = ''; if (!modules || modules.length === 0) { container.innerHTML = '<div>No modules yet</div>'; return; }
    modules.sort((a, b) => (a.position || 0) - (b.position || 0));
    modules.forEach(m => {
        const moduleDiv = document.createElement('div');
        const titleDiv = document.createElement('div');
        titleDiv.textContent = m.title || ''; titleDiv.dataset.action = 'edit-module'; titleDiv.dataset.moduleSlug = m.slug || ''; titleDiv.dataset.moduleTitle = m.title || ''; titleDiv.dataset.modulePosition = m.position || 0; titleDiv.style.fontWeight = '600'; titleDiv.style.cursor = 'pointer'; moduleDiv.appendChild(titleDiv);
        const posDiv = document.createElement('div'); posDiv.textContent = 'Position: ' + (m.position || 0); posDiv.style.color = 'var(--palette-text-shadow)'; posDiv.style.fontSize = '0.9rem'; posDiv.style.marginBottom = '8px'; moduleDiv.appendChild(posDiv);
        const controlsDiv = document.createElement('div'); controlsDiv.style.display = 'flex'; controlsDiv.style.gap = '8px'; controlsDiv.style.marginBottom = '12px';
        const editBtn = document.createElement('button'); editBtn.textContent = '✏️ Edit'; editBtn.dataset.action = 'edit-module'; editBtn.dataset.moduleSlug = m.slug || ''; editBtn.dataset.moduleTitle = m.title || ''; editBtn.dataset.modulePosition = m.position || 0; controlsDiv.appendChild(editBtn);
        const deleteBtn = document.createElement('button'); deleteBtn.textContent = '🗑️ Delete'; deleteBtn.dataset.action = 'delete-module'; deleteBtn.dataset.moduleSlug = m.slug || ''; controlsDiv.appendChild(deleteBtn);
        const addLessonBtn = document.createElement('button'); addLessonBtn.textContent = '➕ Lesson'; addLessonBtn.dataset.action = 'add-lesson'; addLessonBtn.dataset.moduleId = m.id || ''; addLessonBtn.dataset.moduleSlug = m.slug || ''; controlsDiv.appendChild(addLessonBtn);
        moduleDiv.appendChild(controlsDiv);
        if (m.lessons && m.lessons.length > 0) { const ul = document.createElement('ul'); m.lessons.slice().sort((x, y) => (x.position || 0) - (y.position || 0)).forEach(l => { const li = document.createElement('li'); li.style.display = 'flex'; li.style.justifyContent = 'space-between'; li.style.alignItems = 'center'; li.style.marginBottom = '8px'; const left = document.createElement('div'); const lessonTitle = document.createElement('div'); lessonTitle.textContent = l.title || ''; lessonTitle.dataset.action = 'edit-lesson'; lessonTitle.dataset.lessonId = l.id || ''; lessonTitle.dataset.moduleId = m.id || ''; lessonTitle.style.cursor = 'pointer'; lessonTitle.style.fontWeight = '600'; left.appendChild(lessonTitle); const slugSmall = document.createElement('div'); slugSmall.textContent = l.slug || ''; slugSmall.style.fontSize = '0.85rem'; slugSmall.style.color = 'var(--palette-color-tc)'; left.appendChild(slugSmall); li.appendChild(left); const right = document.createElement('div'); right.style.display = 'flex'; right.style.gap = '8px'; const editLessonBtn = document.createElement('button'); editLessonBtn.textContent = '✏️'; editLessonBtn.dataset.action = 'edit-lesson'; editLessonBtn.dataset.lessonId = l.id || ''; editLessonBtn.dataset.moduleId = m.id || ''; right.appendChild(editLessonBtn); const deleteLessonBtn = document.createElement('button'); deleteLessonBtn.textContent = '🗑️'; deleteLessonBtn.dataset.action = 'delete-lesson'; deleteLessonBtn.dataset.lessonId = l.id || ''; right.appendChild(deleteLessonBtn); li.appendChild(right); ul.appendChild(li); }); moduleDiv.appendChild(ul); } else { const none = document.createElement('div'); none.textContent = 'No lessons yet'; none.style.color = 'var(--palette-color-tc)'; none.style.fontStyle = 'italic'; moduleDiv.appendChild(none); }
        container.appendChild(moduleDiv); const hr = document.createElement('hr'); container.appendChild(hr);
    });
}

async function loadModules() {
    const slugEl = document.getElementById('CourseSlug') || document.getElementById('CourseSlugInput');
    const slug = slugEl ? slugEl.value : '';
    const container = document.getElementById('modulesContainer');
    if (!slug) { if (container) container.innerHTML = '<div>Course slug missing</div>'; return; }
    const json = await getJson(`/AdminCourse/GetCourseModules?slug=${encodeURIComponent(slug)}`);
    if (json && json.success) renderModules(json.data || []); else if (container) container.innerHTML = 'Unable to load modules: ' + (json?.message || 'unknown');
}

// ---------- Delegated actions for module/lesson operations ----------
document.addEventListener('click', async function (e) {
    const actionEl = e.target.closest && e.target.closest('[data-action]');
    if (!actionEl) { if (!e.target.closest('.dropdown-menu') && !e.target.closest('.toolbar-btn')) hideAllDropdowns(); return; }
    const action = actionEl.dataset.action;
    try {
        if (action === 'edit-module') {
            document.getElementById('modalModuleSlug').value = actionEl.dataset.moduleSlug || '';
            document.getElementById('modalModuleTitle').value = actionEl.dataset.moduleTitle || '';
            document.getElementById('modalModulePosition').value = actionEl.dataset.modulePosition || 0;
            showModal('moduleModalOverlay');
        } else if (action === 'delete-module') {
            const slug = actionEl.dataset.moduleSlug; if (!slug) { alert('Module identifier missing'); return; } if (!confirm('Delete module and its lessons?')) return; const res = await postUrlEncoded('/AdminCourse/DeleteModule', { moduleSlug: slug }); if (res && res.success) await loadModules(); else alert(res && res.message ? res.message : 'Error deleting module');
        } else if (action === 'add-lesson') {
            const moduleSlug = actionEl.dataset.moduleSlug;
            if (!moduleSlug) { alert('Module id missing'); return; }
            const title = prompt('Lesson title:');
            if (!title) return;
            const lessonSlug = prompt('Lesson slug (optional):') || '';
            const res = await postUrlEncoded('/AdminCourse/AddLesson', { ModuleSlug: moduleSlug, Title: title, Body: "", Slug: lessonSlug });
            if (res && res.success) await loadModules(); else alert(res && res.message ? res.message : 'Error adding lesson');
        } else if (action === 'edit-lesson') {
            const lessonId = actionEl.dataset.lessonId; if (!lessonId) { alert('Lesson id missing'); return; } const json = await getJson(`/AdminCourse/GetLesson?lessonId=${encodeURIComponent(lessonId)}`); if (!json || !json.success) { alert('Unable to load lesson for editing'); return; } openLessonModalWithData(json.data || {});
        } else if (action === 'delete-lesson') {
            const lessonId = actionEl.dataset.lessonId; if (!lessonId) { alert('Lesson id missing'); return; } if (!confirm('Delete lesson?')) return; const res = await postUrlEncoded('/AdminCourse/DeleteLesson', { lessonId }); if (res && res.success) await loadModules(); else alert(res && res.message ? res.message : 'Error deleting lesson');
        }
    } catch (err) { console.error('Action handler error', err); alert('Unexpected error, see console.'); }
});

// ---------- Module modal save/add handlers ----------
const btnSaveModule = document.getElementById('btnSaveModule'); if (btnSaveModule) btnSaveModule.addEventListener('click', async function () { const slug = document.getElementById('modalModuleSlug').value; const title = document.getElementById('modalModuleTitle').value; const position = document.getElementById('modalModulePosition').value || 0; if (!slug) { alert('Module identifier missing'); return; } const res = await postUrlEncoded('/AdminCourse/UpdateModule', { Slug: slug, Title: title, Position: position }); if (res && res.success) { hideModal('moduleModalOverlay'); await loadModules(); } else alert(res && res.message ? res.message : 'Unable to save module'); });
const btnAddModule = document.getElementById('btnAddModule'); if (btnAddModule) btnAddModule.addEventListener('click', async function () { const title = prompt('Module title:'); if (!title) return; const courseSlugEl = document.getElementById('CourseSlug') || document.getElementById('CourseSlugInput'); const courseSlug = courseSlugEl ? courseSlugEl.value : ''; if (!courseSlug) { alert('Course slug missing'); return; } const moduleSlug = prompt('Module slug (optional):') || ''; const res = await postUrlEncoded('/AdminCourse/AddModule', { Slug: moduleSlug, Title: title, CourseSlug: courseSlug }); if (res && res.success) await loadModules(); else alert(res && res.message ? res.message : 'Error adding module'); });

// ---------- image preview for main image ----------
const mainImgEl = document.getElementById('MainImageUrl'); if (mainImgEl) {
    mainImgEl.addEventListener('input', debounce(function (e) {
        const url = e.target.value.trim();
        const wrap = document.getElementById('imagePreviewWrap');
        const img = document.getElementById('imgPreview');
        if (!wrap || !img) return;
        if (!url) { wrap.setAttribute('hidden', ''); img.src = ''; return; }
        img.src = url;
        wrap.removeAttribute('hidden');
    }, 300));
}

// ---------- init ----------
(function init() {
    loadModules();
})();
