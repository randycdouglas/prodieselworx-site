(() => {
  const toggle = document.querySelector('[data-nav-toggle]');
  const links = document.querySelector('[data-nav-links]');

  if (toggle && links) {
    const closeMenu = () => {
      links.classList.remove('open');
      toggle.classList.remove('open');
      toggle.setAttribute('aria-expanded', 'false');
      toggle.setAttribute('aria-label', 'Open menu');
    };

    toggle.addEventListener('click', () => {
      const isOpen = links.classList.toggle('open');
      toggle.classList.toggle('open', isOpen);
      toggle.setAttribute('aria-expanded', String(isOpen));
      toggle.setAttribute('aria-label', isOpen ? 'Close menu' : 'Open menu');
    });

    links.querySelectorAll('a').forEach(link => link.addEventListener('click', closeMenu));
    document.addEventListener('keydown', event => {
      if (event.key === 'Escape') closeMenu();
    });
  }

  const header = document.querySelector('[data-header]');
  const setHeaderState = () => header?.classList.toggle('site-header--scrolled', window.scrollY > 12);
  setHeaderState();
  window.addEventListener('scroll', setHeaderState, { passive: true });

  const revealItems = document.querySelectorAll('.reveal');
  if ('IntersectionObserver' in window && !window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('is-visible');
          observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12 });
    revealItems.forEach(item => observer.observe(item));
  } else {
    revealItems.forEach(item => item.classList.add('is-visible'));
  }


  const contactForm = document.querySelector('[data-contact-form]');
  if (contactForm) {
    const submitButton = contactForm.querySelector('[data-contact-submit]');
    const submitLabel = contactForm.querySelector('[data-submit-label]');
    const status = contactForm.querySelector('[data-contact-status]');
    const fields = contactForm.querySelectorAll('input, textarea');

    const setStatus = (message, type = '') => {
      if (!status) return;
      status.textContent = message;
      status.classList.remove('is-success', 'is-error');
      if (type) status.classList.add(`is-${type}`);
    };

    fields.forEach(field => {
      field.addEventListener('invalid', () => field.setAttribute('aria-invalid', 'true'));
      field.addEventListener('input', () => {
        if (field.checkValidity()) field.removeAttribute('aria-invalid');
      });
    });

    contactForm.addEventListener('submit', async event => {
      event.preventDefault();
      setStatus('');
      fields.forEach(field => field.removeAttribute('aria-invalid'));

      if (!contactForm.checkValidity()) {
        contactForm.reportValidity();
        return;
      }

      const formData = new FormData(contactForm);
      const payload = Object.fromEntries(formData.entries());

      if (submitButton) submitButton.disabled = true;
      if (submitLabel) submitLabel.textContent = 'Sending…';
      setStatus('Sending your message…');

      try {
        const response = await fetch('/api/contact', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
          body: JSON.stringify(payload)
        });

        let data = null;
        try { data = await response.json(); } catch { /* use fallback below */ }

        if (!response.ok) {
          const fallback = response.status === 429
            ? 'Too many messages were submitted from this connection. Please wait a few minutes or call (985) 868-1438.'
            : 'We couldn’t send your message right now. Please try again or call (985) 868-1438.';
          throw new Error(data?.message || fallback);
        }

        contactForm.reset();
        setStatus(data?.message || 'Thanks — your message was sent. We’ll be in touch as soon as possible.', 'success');
      } catch (error) {
        setStatus(error?.message || 'We couldn’t send your message right now. Please call (985) 868-1438.', 'error');
      } finally {
        if (submitButton) submitButton.disabled = false;
        if (submitLabel) submitLabel.textContent = 'Send Message';
      }
    });
  }
})();
