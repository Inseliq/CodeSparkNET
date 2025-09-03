document.addEventListener("DOMContentLoaded", () => {
  const html = document.documentElement;

  function setDevice() {
    const isTouch = "ontouchstart" in window || navigator.maxTouchPoints > 0;
    const deviceWidth = window.screen.width;

    if (isTouch || deviceWidth <= 540) {
      html.setAttribute("data-device", "mobile");
      console.log(`Устройство: mobile (${deviceWidth}px)`);
    } else {
      html.setAttribute("data-device", "desktop");
      console.log(`Устройство: desktop (${deviceWidth}px)`);
    }
  }

  // default
  setDevice();

  window.addEventListener("orientationchange", setDevice);
});