document.addEventListener("DOMContentLoaded", function () {
  const html = document.documentElement;

  if (html.dataset.device === "desktop") {
    const hoverElements = document.querySelectorAll("[hover]");

    hoverElements.forEach(el => {
      el.addEventListener("mouseenter", () => {
        el.classList.add("hover");
      });

      el.addEventListener("mouseleave", () => {
        el.classList.remove("hover");
      });
    });
  }
});