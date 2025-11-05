(function () {
  const loader = document.getElementById('page-loader');
  const body = document.body;

  const connectionTimeout = setTimeout(() => {
    if (!navigator.onLine) {
      showBadLoadMessage('Нет соединения с интернетом');
    } else if (loader) {
      showBadLoadMessage('Плохое интернет соединение');
    }
  }, 6000);

  function showBadLoadMessage(msg) {
    const section = document.createElement('section');
    section.setAttribute('js-bad-load-visible', '');
    const badLoad = document.createElement('div');
    badLoad.setAttribute('js-bad-load', '');
    const article = document.createElement('article');
    article.textContent = msg;
    section.appendChild(badLoad);
    section.appendChild(article);
    loader.appendChild(section);
    const JsLoader = document.querySelector('[js-loader]');
    JsLoader.style.top = '35%';
    section.style.top = '70%';
  }

  window.reactAppReady = function () {
    clearTimeout(connectionTimeout);
    if (loader) {
      loader.style.opacity = '0';
      setTimeout(() => loader.remove(), 400);
    }
    body.classList.remove('not-ready');
    const root = document.getElementById('root');
    if (root) root.classList.add('ready');
  };
})();