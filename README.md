# Pro Diesel Worx LLC — Website

Static marketing site for **Pro Diesel Worx LLC**, a diesel maintenance and repair shop in Lockport, LA.

Plain HTML/CSS — no framework, no build step. Deploys as-is to GitHub Pages.

## Structure

```
prodieselworx-site/
├── index.html        # Home: hero, services, about, CTA
├── contact.html      # Contact info, message form, map
├── css/
│   └── styles.css    # All styling (brand: red / black / white)
└── img/
    └── logo.png      # <-- add the real logo here
```

## Before it goes live — TODO

1. **Add the logo.** Save the Pro Diesel Worx logo as `img/logo.png`.
   (Until then, the nav falls back to a text logo automatically.)
2. **Optional photos.** Drop `img/hero.jpg` (shop/truck background) and
   `img/about.jpg` (about section) for a richer look. Both are optional —
   the site degrades gracefully without them.
3. **Contact form.** The form on `contact.html` needs a handler to actually
   send. Easiest for a static site: create a free form at
   [formspree.io](https://formspree.io) and replace `YOUR_FORM_ID` in
   `contact.html`.
4. **Email address.** No public email was found. If the client has one,
   uncomment the email block in `contact.html` and add it.

## Business info (as published)

- **Phone:** (985) 696-0577
- **Location:** Lockport, LA
- **Hours:** Always open
- **Facebook:** https://www.facebook.com/prodieselworx/

## Local preview

Just open `index.html` in a browser — no server needed.

## Deploy (GitHub Pages)

1. Push this folder to a GitHub repo.
2. Repo **Settings → Pages**.
3. Source: **Deploy from a branch**, branch `main`, folder `/ (root)`.
4. Save. Live in ~1 minute at `https://<username>.github.io/<repo>/`.
