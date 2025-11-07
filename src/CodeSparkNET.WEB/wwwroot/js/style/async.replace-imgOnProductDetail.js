document.addEventListener('DOMContentLoaded', function () {
  const mainImage = document.getElementById('mainProductImage');
  const thumbButtons = document.querySelectorAll('.thumb-btn');

  if (!mainImage || thumbButtons.length === 0) return;

  thumbButtons.forEach(btn => {
    btn.addEventListener('click', () => {
      const thumbImg = btn.querySelector('.thumb-btn__img');
      if (!thumbImg) return;

      const tempSrc = mainImage.src;
      const tempAlt = mainImage.alt;

      mainImage.src = thumbImg.src;
      mainImage.alt = thumbImg.alt;

      thumbImg.src = tempSrc;
      thumbImg.alt = tempAlt;
    });

    btn.addEventListener('mouseenter', () => btn.classList.add('hover'));
    btn.addEventListener('mouseleave', () => btn.classList.remove('hover'));
  });

  const galleryMain = document.querySelector('.gallery__main');
  if (galleryMain) {
    galleryMain.addEventListener('mouseenter', () => galleryMain.classList.add('hover'));
    galleryMain.addEventListener('mouseleave', () => galleryMain.classList.remove('hover'));
  }
});