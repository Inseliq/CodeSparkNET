(function () {
  const SCRIPT_KEY = 'offlineLoaderScript';
  const SCRIPT_VERSION = '1.7';

  function runLoader() {
    const style = document.createElement('style');
    style.textContent = `
[js-load-page] {
  position: fixed;
  inset: 0;
  opacity: 1;
  z-index: 1000;
  display: flex;
  height: 100%;
  width: 100%;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  background: #1f1f1f;

  section {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    transition: .6s ease;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 35px;
    background: #ffd23f30;
    border: 2px #ffd23f solid;
    border-radius: 8px;
    padding: 10px;

    [js-bad-load] {
      width: 60px;
      height: 50px;
      --m: no-repeat linear-gradient(90deg, #000 70%, #0000 0);
      -webkit-mask:
        var(--m) calc(0*100%/4) 100%/calc(100%/5) calc(1*100%/5),
        var(--m) calc(1*100%/4) 100%/calc(100%/5) calc(2*100%/5),
        var(--m) calc(2*100%/4) 100%/calc(100%/5) calc(3*100%/5),
        var(--m) calc(3*100%/4) 100%/calc(100%/5) calc(4*100%/5),
        var(--m) calc(4*100%/4) 100%/calc(100%/5) calc(5*100%/5);
      background: linear-gradient(#ffa216 0 0) left/0% 100% no-repeat #ddd;
      animation: load-not_conntecion 2s infinite steps(6);
    }

    article {
      color: #ffd23f;
      font-weight: 700;
      font-size: 20px;
    }
  }

  [js-loader] {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    transition: .6s ease;
    width: 70px;
    aspect-ratio: 1;
    display: grid;

    &::before,
    &::after {
      content: "";
      grid-area: 1/1;
      --c: no-repeat radial-gradient(farthest-side, #9780e5 92%, #0000);
      background:
        var(--c) 50% 0,
        var(--c) 50% 100%,
        var(--c) 100% 50%,
        var(--c) 0 50%;
      background-size: 20px 20px;
      animation: load-main 1s infinite;
    }

    &::before {
      margin: 4px;
      filter: hue-rotate(45deg);
      background-size: 12px 12px;
      animation-timing-function: linear
    }
  }
}

@keyframes load-main {
  100% {
    transform: rotate(.5turn)
  }
}

@keyframes load-not_conntecion {
  100% {
    background-size: 120% 100%
  }
}
  `;
    document.head.appendChild(style);

    const loadPage = document.createElement('div');
    loadPage.setAttribute('js-load-page', '');

    const loader = document.createElement('div');
    loader.setAttribute('js-loader', '');

    loadPage.appendChild(loader);
    document.body.appendChild(loadPage);
    document.body.style.overflow = 'hidden';

    const start = Date.now();
    const MIN_SHOW = 500;
    const SHOW_ERROR_AFTER = 5000;

    let errorShown = false;

    function showBadLoadMessage(text) {
      if (errorShown) return;
      errorShown = true;

      const section = document.createElement('section');
      section.setAttribute('js-bad-load-visible', '');

      const badLoad = document.createElement('div');
      badLoad.setAttribute('js-bad-load', '');

      const article = document.createElement('article');
      article.textContent = text;

      section.appendChild(badLoad);
      section.appendChild(article);

      loadPage.insertBefore(section, loader);
      loader.style.top = '70%';
    }

    function hideLoader() {
      const elapsed = Date.now() - start;
      const wait = elapsed < MIN_SHOW ? MIN_SHOW - elapsed : 0;
      setTimeout(() => {
        loadPage.style.opacity = '0';
        document.body.style.overflow = '';
        setTimeout(() => { if (loadPage.parentNode) loadPage.remove(); }, 600);
      }, wait);
    }

    window.addEventListener('load', hideLoader);

    const t = setTimeout(() => {
      if (document.readyState !== 'complete') {
        const msg = navigator.onLine ? 'Связь с сервером потеряна' : 'Плохое интернет соединение';
        showBadLoadMessage(msg);
      }
    }, SHOW_ERROR_AFTER);

    window.addEventListener('error', () => {
      const msg = navigator.onLine ? 'Связь с сервером потеряна' : 'Плохое интернет соединение';
      showBadLoadMessage(msg);
      clearTimeout(t);
    }, true);

    window.addEventListener('offline', () => showBadLoadMessage('Нет подключения к интернету'));
  }

  const saved = localStorage.getItem(SCRIPT_KEY);
  const savedVersion = localStorage.getItem(SCRIPT_KEY + '_ver');

  if (!saved || savedVersion !== SCRIPT_VERSION) {
    localStorage.setItem(SCRIPT_KEY, '(' + runLoader.toString() + ')();');
    localStorage.setItem(SCRIPT_KEY + '_ver', SCRIPT_VERSION);
    runLoader();
  } else {
    try {
      eval(saved);
    } catch (e) {
      runLoader();
    }
  }
})();