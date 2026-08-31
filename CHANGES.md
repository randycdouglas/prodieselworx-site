# Pro Diesel Worx Redesign — Changes

## Visual redesign
- Rebuilt the homepage around an industrial diesel aesthetic using the existing red/black/white logo palette.
- Added a stronger hero, capability rail, service-card system, branded industrial graphic section, service steps and conversion-focused CTA band.
- Reworked typography, spacing, responsive behavior, shadows, borders and mobile presentation.
- Added subtle grid / steel-inspired visual texture without introducing external image dependencies.

## Conversion improvements
- Made the phone number the primary action throughout the site.
- Added a persistent mobile click-to-call bar.
- Added an "Always Open" utility bar and clearer service/location messaging.
- Replaced the nonfunctional placeholder Formspree contact form with contact methods that actually work today: phone and Facebook.
- Added a "before you call" checklist to help customers provide useful service information.

## UX / accessibility
- Added a keyboard-accessible mobile menu with `aria-expanded` handling and Escape-to-close.
- Added skip navigation.
- Added reduced-motion support.
- Added better focus-safe semantic structure and responsive layouts.
- Added lazy-loaded map embed.

## SEO / sharing
- Improved page titles and meta descriptions.
- Added canonical URLs.
- Added Open Graph metadata.
- Added LocalBusiness structured data to the homepage.
- Added `robots.txt` and `sitemap.xml`.

## Added files
- `web/wwwroot/js/site.js`
- `web/wwwroot/robots.txt`
- `web/wwwroot/sitemap.xml`
- `CHANGES.md`

## Resend contact form (follow-up)
- Re-added a full contact form to `contact.html` with name, phone, optional email, optional vehicle/equipment details, and message fields.
- Added a same-origin `POST /api/contact` endpoint in ASP.NET Core.
- Integrated the endpoint with Resend's `POST /emails` API without exposing the API key to browser JavaScript.
- Set the visitor's email as `Reply-To` when provided so the shop can reply directly from its inbox.
- Added HTML + plain-text email formatting for incoming leads.
- Added server-side validation, output encoding, honeypot bot filtering, and per-IP rate limiting.
- Added accessible inline success/error status handling and loading state to the form.
- Added responsive styling for the new form.
- Documented the required production environment variables and Resend sender-domain verification.


## 2026-08-31 client contact update
- Updated business phone to **(985) 868-1438** across calls-to-action, metadata, contact-form fallback messages, and server responses.
- Updated business location to **1737 Grand Caillou Rd, Houma, LA 70363**.
- Updated embedded Google map and map links to the exact shop address.
- Updated LocalBusiness structured data with street address and ZIP code.

## Production configuration security
- Added a local-only `web/web.config.production` for Resend/contact-form secrets.
- Added `web/web.config.production` and `*.csproj.user` to `.gitignore`.
- Kept the committed `web/web.config` secret-free.
- Added an MSBuild post-publish target that automatically replaces the published
  `web.config` with the ignored `web.config.production` when publishing locally.
