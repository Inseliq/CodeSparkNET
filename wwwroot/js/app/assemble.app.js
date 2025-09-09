document.addEventListener("DOMContentLoaded", function () {
  const html = document.documentElement;

  if (html.dataset.device === "desktop") {
    document.addEventListener("mouseover", e => {
      const el = e.target.closest("[hover]");
      if (el) {
        el.classList.add("hover");
      }
    });

    document.addEventListener("mouseout", e => {
      const el = e.target.closest("[hover]");
      if (el) {
        el.classList.remove("hover");
      }
    });
  }
});
