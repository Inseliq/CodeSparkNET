const TITLE_MAP = {
  '/': 'CodeSpark',
  '/privacy': 'Политика',
};

export function setTitle(pathname, basename = '') {
  const path = basename && pathname.startsWith(basename)
    ? pathname.slice(basename.length) || '/'
    : pathname;

  const title = TITLE_MAP[path] || 'CodeSpark';
  document.title = title;
}