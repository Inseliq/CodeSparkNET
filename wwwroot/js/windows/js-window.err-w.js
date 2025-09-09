document.addEventListener("DOMContentLoaded", () => {
  function createModal() {
    const modal = document.createElement("div");
    modal.setAttribute("js-window-err_w", "");
    modal.className = "js-window";

    modal.innerHTML = `
      <div class="description" hover>
        <button close-btn-err_w hover>×</button>
        <span class="loader"></span>
        <span class="desc">Данный модуль находится в разработке и будет доступен позже.</span>
      </div>
    `;

    document.body.appendChild(modal);

    // плавное появление
    setTimeout(() => modal.classList.add("visible"), 100);

    // закрытие
    function closeModal() {
      modal.classList.remove("visible");
      modal.classList.add("hidden");
      setTimeout(() => modal.remove(), 300);
    }

    modal.addEventListener("click", e => {
      if (e.target.hasAttribute("js-window-err_w")) closeModal();
    });

    modal.querySelector("[close-btn-err_w]").addEventListener("click", closeModal);
  }

  document.querySelectorAll("[err-w]").forEach(btn => {
    btn.addEventListener("click", createModal);
  });
});