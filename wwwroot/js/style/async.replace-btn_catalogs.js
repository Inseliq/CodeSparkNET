document.addEventListener("DOMContentLoaded", function () {
  const catalogSectionOn = document.getElementById("CatalogSectionOn");
  const miniApp = document.getElementById("miniApp");
  const catalogsSections = document.getElementById("CatalogsSections");

  if (catalogSectionOn) {
    // Если есть CatalogSectionOn
    if (miniApp) miniApp.style.display = "none";
    if (catalogsSections) catalogsSections.style.display = "inline-block";
  } else {
    // Если нет CatalogSectionOn
    if (miniApp) miniApp.style.display = "inline-block";
    if (catalogsSections) catalogsSections.style.display = "none";
  }
});