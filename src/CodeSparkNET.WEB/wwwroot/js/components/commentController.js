document.addEventListener('DOMContentLoaded', () => {
  const existing = document.querySelector('link[href="/css/Components/comment-component.css"]');
  if (existing) return;

  const link = document.createElement('link');
  link.rel = 'stylesheet';
  link.type = 'text/css';
  link.href = '/css/Components/comment-component.css';

  document.head.appendChild(link);

  console.log('[app-loader] component "Comments" loaded');
});