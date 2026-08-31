# Pro Diesel Worx LLC — Website

ASP.NET Core 10 static marketing site for **Pro Diesel Worx LLC**, a diesel maintenance and repair business in Lockport, Louisiana.

## Current design

The site was redesigned with a stronger industrial diesel look while keeping the existing red / black / white brand and existing business details.

### Included

- Responsive homepage and contact page
- Sticky navigation with accessible mobile menu
- Click-to-call CTAs throughout the site
- Mobile sticky call bar
- Redesigned service cards and diesel capability sections
- Improved contact experience (phone / location / hours / Facebook)
- Embedded Lockport map
- Open Graph / social metadata
- LocalBusiness structured data
- Canonical URLs
- `robots.txt` and `sitemap.xml`
- Reduced-motion accessibility support
- No JavaScript framework or third-party UI dependency

## Business information currently published

- **Phone:** (985) 696-0577
- **Location:** Lockport, LA
- **Hours:** Always Open
- **Facebook:** https://www.facebook.com/prodieselworx/
- **Website:** https://prodieselworx.com/

## Project structure

```text
ProDieselWorx/
├── ProDieselWorx.slnx
├── README.md
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

The application is a minimal ASP.NET Core host that serves the static files from `wwwroot`. Publish the `web/ProDieselWorx.Web.csproj` project to the MonsterASP website.

## Future improvements that require client information

These were intentionally not invented in the redesign. Add them when the client provides accurate information:

1. Exact street address (if the shop wants it public)
2. Public business email address
3. Real shop / truck / engine photos
4. Exact makes, engines, truck classes, or equipment types serviced
5. Warranty or workmanship claims
6. Customer testimonials / reviews
7. A working contact-form destination or email delivery service

The old Formspree placeholder form was removed rather than publishing a form that cannot actually deliver messages. Phone and Facebook are currently the working contact channels shown on the site.
