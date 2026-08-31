# Pro Diesel Worx LLC — Website

ASP.NET Core 10 marketing site for **Pro Diesel Worx LLC**, a diesel maintenance and repair business in Houma, Louisiana.

## Current design

The site was redesigned with a stronger industrial diesel look while keeping the existing red / black / white brand and existing business details.

### Included

- Responsive homepage and contact page
- Sticky navigation with accessible mobile menu
- Click-to-call CTAs throughout the site
- Mobile sticky call bar
- Redesigned service cards and diesel capability sections
- **Working contact form delivered through Resend**
- Server-side form validation and HTML encoding
- Honeypot spam trap and per-IP rate limiting
- Embedded map to 1737 Grand Caillou Rd, Houma, LA 70363
- Open Graph / social metadata
- LocalBusiness structured data
- Canonical URLs
- `robots.txt` and `sitemap.xml`
- Reduced-motion accessibility support
- No JavaScript framework or third-party UI dependency

## Business information currently published

- **Phone:** (985) 868-1438
- **Address:** 1737 Grand Caillou Rd, Houma, LA 70363
- **Hours:** Always Open
- **Facebook:** https://www.facebook.com/prodieselworx/
- **Website:** https://prodieselworx.com/

## Resend contact-form setup

The browser submits the form to `POST /api/contact`. The ASP.NET Core server validates the request and sends the email through Resend's server-side API. **The Resend API key is never sent to the browser.**

Configure these three values on the production server:

```text
RESEND_API_KEY=re_xxxxxxxxxxxxxxxxx
RESEND_FROM_EMAIL=Pro Diesel Worx Website <website@prodieselworx.com>
CONTACT_TO_EMAIL=the-address-that-should-receive-leads@example.com
```

The application also supports standard ASP.NET configuration keys:

```text
Resend:ApiKey
Resend:FromEmail
Contact:ToEmail
```

### Sender-domain requirement

`RESEND_FROM_EMAIL` must use a sender/domain that is verified in the Resend account.

If you want the sender to be `website@prodieselworx.com`, add `prodieselworx.com` in Resend and give the client's DNS administrator the verification records Resend generates. Those records are **in addition to** the website A/CNAME records used to point the site at MonsterASP.

Alternatively, the form can send from any other domain already verified in the same Resend account. The customer's email, when supplied, is set as the email's `Reply-To`, so clicking Reply in the client's inbox responds directly to the customer.

### Recommended Resend API key

Create a dedicated API key with **Sending access only**, ideally restricted to the sender domain used by this website. Do not commit the key to Git or put it in `wwwroot`/JavaScript.

### Spam / abuse protection

The endpoint includes:

- hidden honeypot field
- server-side length and email validation
- HTML encoding of customer-provided values
- 5 submissions per IP address per 10 minutes
- no exposure of Resend credentials to the client

If spam ever becomes significant, a CAPTCHA/Turnstile challenge can be added later without replacing the Resend integration.

## Project structure

```text
ProDieselWorx/
├── ProDieselWorx.slnx
├── README.md
├── CHANGES.md
└── web/
    ├── ProDieselWorx.Web.csproj
    ├── Program.cs
    ├── web.config
    └── wwwroot/
        ├── index.html
        ├── contact.html
        ├── robots.txt
        ├── sitemap.xml
        ├── css/styles.css
        ├── js/site.js
        └── img/logo.png
```

## Publishing

Publish `web/ProDieselWorx.Web.csproj` to the MonsterASP website. After publishing, configure the three Resend/contact settings above and restart the site/application so the environment settings are loaded.

## Future improvements that require client information

These were intentionally not invented. Add them when the client provides accurate information:

1. Public business email address
3. Real shop / truck / engine photos
4. Exact makes, engines, truck classes, or equipment types serviced
5. Warranty or workmanship claims
6. Customer testimonials / reviews

## Production Resend configuration

The committed `web/web.config` intentionally contains no secrets.

For a production publish, edit the local file:

`web/web.config.production`

Fill in:

- `RESEND_API_KEY` — the Resend API key for `prodieselworx.com`
- `RESEND_FROM_EMAIL` — defaults to `Pro Diesel Worx Website <website@prodieselworx.com>`
- `CONTACT_TO_EMAIL` — the mailbox that should receive contact-form submissions

`web.config.production` is ignored by Git and must never be committed.

When you publish the project locally with Visual Studio or `dotnet publish`, the
project automatically uses `web.config.production` as the `web.config` in the
publish output. A build/deploy performed only from GitHub will not have access to
this ignored file, so use a local publish for the secret-bearing production build
unless the hosting platform stores these values separately.
