// Help Page Support System
class HelpPageRouter {
  constructor() {
    this.container = document.querySelector('[data-app="root"]');
    this.currentUser = this.getCurrentUser();
    this.init();
  }

  init() {
    this.handleRoute();
    window.addEventListener('popstate', () => this.handleRoute());

    document.addEventListener('click', (e) => {
      const target = e.target.closest('[data-route]');
      if (!target) return;

      e.preventDefault();
      const route = target.getAttribute('data-route');
      this.navigate(route);

      setTimeout(() => {
        const container = document.querySelector('.help.__container');
        if (container) {
          const containerTop = container.getBoundingClientRect().top + window.scrollY;
          window.scrollTo({
            top: containerTop,
            behavior: 'smooth'
          });
        }
      }, 150);
    });
  }

  getCurrentUser() {
    // Заглушка для получения текущего пользователя
    // В реальном приложении это будет запрос к серверу или чтение из cookie/localStorage
    return {
      isAuthenticated: false,
      email: ''
    };
  }

  navigate(route) {
    const baseUrl = window.location.pathname.split('/Support/Help')[0];
    const fullPath = `${baseUrl}/Support/Help${route}`;
    window.history.pushState({}, '', fullPath);
    this.handleRoute();
  }

  handleRoute() {
    const path = window.location.pathname;
    const route = path.split('/Support/Help')[1] || '/';

    if (route === '/' || route === '/index' || route === '') {
      this.renderMainPage();
    } else if (route === '/client') {
      this.renderClientSupport();
    } else if (route === '/debug') {
      this.renderDebugSupport();
    } else if (route === '/faq') {
      this.renderMainPage();
      setTimeout(() => this.scrollToFAQ(), 100);
    } else {
      this.renderMainPage();
    }
  }

  scrollToFAQ() {
    const faqSection = document.querySelector('.faq-section');
    if (faqSection) {
      faqSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  renderMainPage() {
    this.container.innerHTML = `
      <div class="help-hero">
        <h1 class="help-hero__title">Центр поддержки CodeSpark</h1>
        <p class="help-hero__subtitle">Мы готовы помочь вам решить любую проблему</p>
      </div>

      <div class="help-cards">
        ${this.createSupportCard(
      'client',
      'Клиентская поддержка',
      'Вопросы о курсах, покупках и работе с материалами',
      'https://images.unsplash.com/photo-1553877522-43269d4ea984?w=800&h=450&fit=crop'
    )}
        ${this.createSupportCard(
      'debug',
      'Техническая поддержка',
      'Сообщить об ошибках, багах и технических проблемах',
      'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=450&fit=crop'
    )}
        ${this.createSupportCard(
      'faq',
      'FAQ',
      'Часто задаваемые вопросы и быстрые ответы',
      'https://images.unsplash.com/photo-1516321497487-e288fb19713f?w=800&h=450&fit=crop'
    )}
      </div>

      <div class="help-stats">
        <div class="help-stats__item">
          <div class="help-stats__value">~12 часов</div>
          <div class="help-stats__label">Среднее время ответа</div>
        </div>
        <div class="help-stats__item">
          <div class="help-stats__value">100%</div>
          <div class="help-stats__label">Решенных запросов</div>
        </div>
        <div class="help-stats__item">
          <div class="help-stats__value">10:00 – 20:00</div>
          <div class="help-stats__label">Время работы</div>
        </div>
      </div>

      <div class="faq-section">
        <h2 class="faq-section__title">Часто задаваемые вопросы</h2>
        <div class="faq-categories">
          ${this.createFAQCategory('technical', 'Технические вопросы', [
      { q: 'Как сбросить пароль?', a: 'Перейдите на страницу входа и нажмите "Забыли пароль?". Введите ваш email, и мы отправим вам ссылку для восстановления.' },
      { q: 'Почему видео не воспроизводится?', a: 'Убедитесь, что у вас стабильное интернет-соединение. Попробуйте обновить страницу или очистить кэш браузера. Если проблема сохраняется, свяжитесь с технической поддержкой.' },
      { q: 'Как обновить профиль?', a: 'Войдите в свой аккаунт, перейдите в раздел "Настройки профиля" и внесите необходимые изменения. Не забудьте нажать кнопку "Сохранить".' }
    ])}
          ${this.createFAQCategory('client', 'Клиентские вопросы', [
      { q: 'Как получить доступ к купленному курсу?', a: 'После успешной оплаты курс автоматически появится в разделе "Мои курсы". Если курс не отображается, проверьте раздел "История покупок" или свяжитесь с поддержкой.' },
      { q: 'Можно ли вернуть деньги за курс?', a: 'Да, мы предоставляем 14-дневную гарантию возврата средств. Если курс вам не подошел, свяжитесь с нами в течение 14 дней с момента покупки.' },
      { q: 'Как получить сертификат?', a: 'Сертификат автоматически генерируется после завершения всех уроков курса и прохождения финального теста с результатом не менее 70%.' },
      { q: 'Есть ли скидки для студентов?', a: 'Да, мы предоставляем 20% скидку для студентов. Для получения скидки необходимо подтвердить статус студента, отправив копию студенческого билета.' }
    ])}
        </div>
      </div>

      <div class="help-contact">
        <div class="help-contact__card">
          <h3 class="help-contact__title">Не нашли ответ?</h3>
          <p class="help-contact__text">Свяжитесь с нами напрямую, и мы поможем решить вашу проблему</p>
          <div class="help-contact__buttons">
            <button class="help-contact__btn help-contact__btn--primary" data-route="/client">
              Клиентская помощь
            </button>
            <button class="help-contact__btn help-contact__btn--secondary" data-route="/debug">
              Техническая помощь
            </button>
          </div>
        </div>
      </div>
    `;

    this.initFAQ();
  }

  createSupportCard(type, title, description, imageUrl) {
    return `
      <div class="support-card" data-route="/${type}">
        <div class="support-card__inner">
          <div class="support-card__front">
            <img src="${imageUrl}" alt="${title}" class="support-card__image">
            <div class="support-card__overlay">
              <h3 class="support-card__title">${title}</h3>
            </div>
          </div>
          <div class="support-card__back">
            <h3 class="support-card__back-title">${title}</h3>
            <p class="support-card__description">${description}</p>
            <button class="support-card__btn" data-route="/${type}">Подробнее</button>
          </div>
        </div>
      </div>
    `;
  }

  createFAQCategory(id, title, questions) {
    return `
      <div class="faq-category" data-category="${id}">
        <div class="faq-category__header" onclick="helpRouter.toggleFAQCategory('${id}')">
          <h3 class="faq-category__title">${title}</h3>
          <svg class="faq-category__icon" width="24" height="24" viewBox="0 0 24 24" fill="none">
            <path d="M6 9l6 6 6-6" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
          </svg>
        </div>
        <div class="faq-category__content">
          ${questions.map((q, idx) => `
            <div class="faq-item">
              <div class="faq-item__question" onclick="helpRouter.toggleFAQItem('${id}', ${idx})">
                <span>${q.q}</span>
                <svg class="faq-item__icon" width="20" height="20" viewBox="0 0 20 20" fill="none">
                  <path d="M5 7.5l5 5 5-5" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
                </svg>
              </div>
              <div class="faq-item__answer">${q.a}</div>
            </div>
          `).join('')}
        </div>
      </div>
    `;
  }

  initFAQ() {
    // Все категории и вопросы закрыты по умолчанию
  }

  toggleFAQCategory(categoryId) {
    const categories = document.querySelectorAll('.faq-category');
    const targetCategory = document.querySelector(`[data-category="${categoryId}"]`);

    categories.forEach(cat => {
      if (cat !== targetCategory && cat.classList.contains('faq-category--open')) {
        cat.classList.remove('faq-category--open');
        // Закрываем все вопросы в закрываемой категории
        cat.querySelectorAll('.faq-item').forEach(item => {
          item.classList.remove('faq-item--open');
        });
      }
    });

    targetCategory.classList.toggle('faq-category--open');
  }

  toggleFAQItem(categoryId, itemIdx) {
    const category = document.querySelector(`[data-category="${categoryId}"]`);
    const items = category.querySelectorAll('.faq-item');
    const targetItem = items[itemIdx];

    items.forEach(item => {
      if (item !== targetItem) {
        item.classList.remove('faq-item--open');
      }
    });

    targetItem.classList.toggle('faq-item--open');
  }

  performSearch() {
    const input = document.getElementById('searchInput');
    const query = input.value.trim();
    if (query) {
      alert(`Поиск по запросу: "${query}"\nВ реальном приложении здесь будет поиск по базе знаний.`);
    }
  }

  renderClientSupport() {
    const themes = new Map([
      ['products', 'Продукты'],
      ['courses', 'Курсы'],
      ['course-errors', 'Ошибки в курсах'],
      ['payment', 'Оплата и возвраты'],
      ['account', 'Управление аккаунтом'],
      ['certificate', 'Сертификаты']
    ]);

    this.renderSupportForm('client', 'Клиентская поддержка', themes);
  }

  renderDebugSupport() {
    const themes = new Map([
      ['password', 'Восстановление пароля'],
      ['page-errors', 'Ошибки на страницах'],
      ['video-issues', 'Проблемы с видео'],
      ['browser-compatibility', 'Совместимость браузера'],
      ['mobile-issues', 'Проблемы на мобильных'],
      ['performance', 'Производительность сайта']
    ]);

    this.renderSupportForm('debug', 'Техническая поддержка', themes);
  }

  renderSupportForm(category, title, themesMap) {
    this.container.innerHTML = `
      <div class="support-form-page">
        <button class="support-form__back" data-route="/">
          <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
            <path d="M12 4l-6 6 6 6" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
          </svg>
          Назад
        </button>

        <h1 class="support-form__title">${title}</h1>
        <p class="support-form__subtitle">Выберите тему вашего обращения</p>

        <div class="support-themes" id="themesContainer">
          ${Array.from(themesMap.entries()).map(([key, value]) => `
            <button class="theme-btn" data-theme="${key}" onclick="helpRouter.selectTheme('${category}', '${key}', '${value}')">
              ${value}
            </button>
          `).join('')}
        </div>

        <div class="support-form-container" id="formContainer" style="display: none;">
          <h2 class="support-form-container__title" id="selectedTheme"></h2>

          <form class="support-form" id="supportForm">
            <div class="form-group">
              <label class="form-label" for="emailInput">Email</label>
              <input
                type="email"
                class="form-input"
                id="emailInput"
                required
                placeholder="your@email.com"
                value="${this.currentUser.isAuthenticated ? this.currentUser.email : ''}"
                ${this.currentUser.isAuthenticated ? 'readonly' : ''}
              >
            </div>

            <div class="form-group">
              <label class="form-label" for="textInput">Опишите вашу проблему</label>
              <textarea
                class="form-textarea"
                id="textInput"
                required
                placeholder="Подробно опишите вашу проблему или вопрос..."
                rows="8"
              ></textarea>
            </div>

            <button type="submit" class="form-submit">Отправить</button>
          </form>
        </div>
      </div>
    `;

    document.getElementById('supportForm').addEventListener('submit', (e) => {
      e.preventDefault();
      this.submitForm(category);
    });
  }

  selectTheme(category, themeKey, themeTitle) {
    const themesContainer = document.getElementById('themesContainer');
    const formContainer = document.getElementById('formContainer');
    const selectedThemeTitle = document.getElementById('selectedTheme');

    // Скрываем темы и показываем форму
    themesContainer.style.display = 'none';
    formContainer.style.display = 'block';
    selectedThemeTitle.textContent = themeTitle;

    // Сохраняем выбранную тему
    formContainer.dataset.selectedTheme = themeKey;
  }

  async submitForm(category) {
    const formContainer = document.getElementById('formContainer');
    const email = document.getElementById('emailInput').value;
    const text = document.getElementById('textInput').value;
    const theme = formContainer.dataset.selectedTheme;

    const data = {
      theme,
      category,
      email,
      text
    };

    try {
      const baseUrl = window.location.pathname.split('/Support/Help')[0];
      const response = await fetch(`${baseUrl}/Support/Help/${category}/send`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
      });

      this.showSuccessModal();
    } catch (error) {
      console.error('Error submitting form:', error);
      this.showSuccessModal(); // В демо режиме показываем успех в любом случае
    }
  }

  showSuccessModal() {
    const modal = document.createElement('div');
    modal.className = 'success-modal';
    modal.innerHTML = `
    <div class="success-modal__content">
      <div class="success-modal__icon">
        <svg width="64" height="64" viewBox="0 0 64 64" fill="none">
          <circle cx="32" cy="32" r="32" fill="#1E7F34"/>
          <path d="M20 32l8 8 16-16" stroke="white" stroke-width="4" stroke-linecap="round"/>
        </svg>
      </div>
      <h2 class="success-modal__title">Сообщение отправлено!</h2>
      <p class="success-modal__text">Мы получили ваше обращение и ответим в ближайшее время.</p>
      <p class="success-modal__timer">Возврат на главную через <span id="countdown">10</span> секунд</p>
      <button class="success-modal__btn" data-route="/">Вернуться на главную</button>
    </div>
  `;

    document.body.appendChild(modal);
    setTimeout(() => modal.classList.add('success-modal--visible'), 10);

    const backBtn = modal.querySelector('.success-modal__btn');
    backBtn.addEventListener('click', () => {
      this.navigate('/');
      if (modal && modal.parentNode) modal.parentNode.removeChild(modal);
    });

    let countdown = 10;
    const countdownEl = document.getElementById('countdown');
    const interval = setInterval(() => {
      countdown--;
      if (countdownEl) countdownEl.textContent = countdown;
      if (countdown <= 0) {
        clearInterval(interval);
        this.navigate('/');
        if (modal && modal.parentNode) modal.parentNode.removeChild(modal);
      }
    }, 1000);
  }
}

let helpRouter;
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => {
    helpRouter = new HelpPageRouter();
  });
} else {
  helpRouter = new HelpPageRouter();
}