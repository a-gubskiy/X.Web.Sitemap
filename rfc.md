# Sitemaps Protocol (sitemaps.org)

This document describes the XML schema for the Sitemap protocol.

Jump to:

- [XML tag definitions](#xml-tag-definitions)
- [Entity escaping](#entity-escaping)
- [Using Sitemap index files](#using-sitemap-index-files-to-group-multiple-sitemap-files)
- [Other Sitemap formats](#other-sitemap-formats)
- [Sitemap file location](#sitemap-file-location)
- [Validating your Sitemap](#validating-your-sitemap)
- [Extending the Sitemaps protocol](#extending-the-sitemaps-protocol)
- [Informing search engine crawlers](#informing-search-engine-crawlers)

## Overview

The Sitemap protocol format consists of XML tags. All data values in a Sitemap must be entity-escaped. The file itself must be UTF-8 encoded.

Key requirements:

- The Sitemap must begin with an opening `<urlset>` tag and end with a closing `</urlset>` tag.
- The `<urlset>` tag must specify the namespace (protocol standard).
- Include a `<url>` entry for each URL (parent tag).
- Include a `<loc>` child entry for each `<url>` parent tag.
- All other tags are optional; support for optional tags may vary among search engines.
- All URLs in a Sitemap must belong to a single host (for example, `www.example.com` or `store.example.com`).

## Sample XML Sitemap (single URL)

The following example shows a Sitemap that contains one URL and uses the optional tags:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
   <url>
      <loc>http://www.example.com/</loc>
      <lastmod>2005-01-01</lastmod>
      <changefreq>monthly</changefreq>
      <priority>0.8</priority>
   </url>
</urlset>
```

Also see the example with multiple URLs below.

## XML tag definitions

The available XML tags are described below.

| Tag | Required? | Description |
|---|---:|---|
| `<urlset>` | required | Encapsulates the file and references the current protocol standard. |
| `<url>` | required | Parent tag for each URL entry. Remaining tags are children of this tag. |
| `<loc>` | required | URL of the page. Must begin with the protocol (e.g. `http`) and be under 2,048 characters. |
| `<lastmod>` | optional | Date of last modification of the page. Use W3C Datetime format (YYYY-MM-DD or full datetime). This should reflect the page's last modification time, not the sitemap generation time. |
| `<changefreq>` | optional | How frequently the page is likely to change. Valid values: `always`, `hourly`, `daily`, `weekly`, `monthly`, `yearly`, `never`. This is a hint to crawlers, not a command. |
| `<priority>` | optional | Priority of this URL relative to other URLs on the site, from `0.0` to `1.0`. Default is `0.5`. Priority is relative only within your site and does not affect ranking across sites. |

### Notes on `changefreq`

- `always` — documents that change on every access.
- `never` — archived URLs that are not expected to change.

Search engines may ignore these hints or use them differently.

## Entity escaping

Your Sitemap file must be UTF-8 encoded. As with all XML files, data values (including URLs) must use entity escape codes for the following characters:

| Character | Escape Code |
|---|---|
| Ampersand `&` | `&amp;` |
| Single quote `'` | `&apos;` |
| Double quote `"` | `&quot;` |
| Greater than `>` | `&gt;` |
| Less than `<` | `&lt;` |

In addition, all URLs (including the URL of your Sitemap) must be URL-escaped according to RFC-3986 (URIs) and RFC-3987 (IRIs).

Examples:

- Original: `http://www.example.com/ümlat.php&q=name`
- ISO-8859-1 encoded and URL-escaped: `http://www.example.com/%FCmlat.php&q=name`
- UTF-8 encoded and URL-escaped: `http://www.example.com/%C3%BCmlat.php&q=name`
- Entity-escaped: `http://www.example.com/%C3%BCmlat.php&amp;q=name`

## Sample XML Sitemap (multiple URLs)

Example containing several URLs with different optional tags:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
   <url>
      <loc>http://www.example.com/</loc>
      <lastmod>2005-01-01</lastmod>
      <changefreq>monthly</changefreq>
      <priority>0.8</priority>
   </url>
   <url>
      <loc>http://www.example.com/catalog?item=12&amp;desc=vacation_hawaii</loc>
      <changefreq>weekly</changefreq>
   </url>
   <url>
      <loc>http://www.example.com/catalog?item=73&amp;desc=vacation_new_zealand</loc>
      <lastmod>2004-12-23</lastmod>
      <changefreq>weekly</changefreq>
   </url>
   <url>
      <loc>http://www.example.com/catalog?item=74&amp;desc=vacation_newfoundland</loc>
      <lastmod>2004-12-23T18:00:15+00:00</lastmod>
      <priority>0.3</priority>
   </url>
   <url>
      <loc>http://www.example.com/catalog?item=83&amp;desc=vacation_usa</loc>
      <lastmod>2004-11-23</lastmod>
   </url>
</urlset>
```

## Using Sitemap index files (to group multiple sitemap files)

If you need more than 50,000 URLs or larger than 50MB uncompressed, split your site into multiple Sitemap files. Each Sitemap file must:

- Contain at most 50,000 URLs and be no larger than 50MB (52,428,800 bytes) uncompressed.
- Optionally be compressed with gzip (the uncompressed size limit still applies).

When you have multiple Sitemap files, list them in a Sitemap index file. Sitemap index files may list up to 50,000 Sitemaps and follow the same size limits.

Sitemap index requirements:

- Begin with `<sitemapindex>` and end with `</sitemapindex>`.
- Include a `<sitemap>` entry for each Sitemap (parent tag).
- Include a `<loc>` child entry for each `<sitemap>`.
- Optional `<lastmod>` is available to indicate the Sitemap's modification time.
- Sitemap index files must be UTF-8 encoded and can only list Sitemaps on the same host as the index file.

### Sample XML Sitemap Index

```xml
<?xml version="1.0" encoding="UTF-8"?>
<sitemapindex xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
   <sitemap>
      <loc>http://www.example.com/sitemap1.xml.gz</loc>
      <lastmod>2004-10-01T18:23:17+00:00</lastmod>
   </sitemap>
   <sitemap>
      <loc>http://www.example.com/sitemap2.xml.gz</loc>
      <lastmod>2005-01-01</lastmod>
   </sitemap>
</sitemapindex>
```

Note: Sitemap URLs must be entity escaped like other XML values.

### Sitemap index XML tag definitions

| Tag | Required? | Description |
|---|---:|---|
| `<sitemapindex>` | required | Encapsulates information about all Sitemaps in the file. |
| `<sitemap>` | required | Encapsulates information about an individual Sitemap. |
| `<loc>` | required | Identifies the location of the Sitemap (can point to a Sitemap, Atom, RSS, or text file). |
| `<lastmod>` | optional | Time the corresponding Sitemap file was modified (W3C Datetime). Useful for incremental fetching. |

## Other Sitemap formats

In addition to the XML protocol, you can provide:

- Syndication feeds (RSS 2.0 or Atom 0.3 / 1.0) — useful when a site already has a feed. Search engines extract the URL from the `<link>` field and optionally the modified date from `<pubDate>` (RSS) or `<updated>` (Atom).
- Plain text files — one URL per line. Guidelines for text files:
   - One URL per line (no embedded newlines).
   - Fully specify URLs including `http`/`https`.
   - Up to 50,000 URLs and 50MB uncompressed per file.
   - Use UTF-8 encoding and no header/footer information.
   - Can be gzip-compressed.

Sample text entries:

```
http://www.example.com/catalog?item=1

http://www.example.com/catalog?item=11
```

## Sitemap file location

The path of a Sitemap determines which URLs may be included. A Sitemap at `http://example.com/catalog/sitemap.xml` may include URLs that begin with `http://example.com/catalog/` but not `http://example.com/images/`.

Examples considered valid in `http://example.com/catalog/sitemap.xml`:

```
http://example.com/catalog/show?item=23
http://example.com/catalog/show?item=233&user=3453
```

Examples not valid:

```
http://example.com/image/show?item=23
http://example.com/image/show?item=233&user=3453
https://example.com/catalog/page1.php
```

All URLs in the Sitemap must use the same protocol and host as the Sitemap location. It is strongly recommended to place your Sitemap at the root of your web server (for example, `http://example.com/sitemap.xml`).

If a Sitemap is served from a URL with a port (for example `http://www.example.com:100/sitemap.xml`), then each URL in the sitemap must include that port.

## Sitemaps & Cross Submits

To submit Sitemaps for multiple hosts from a single host you must prove ownership of the target hosts. Example setup:

- `www.host1.com` — `sitemap-host1.xml`
- `www.host2.com` — `sitemap-host2.xml`
- `www.host3.com` — `sitemap-host3.xml`

If you host the three sitemaps on `www.sitemaphost.com`, the sitemap URLs might be:

```
http://www.sitemaphost.com/sitemap-host1.xml
http://www.sitemaphost.com/sitemap-host2.xml
http://www.sitemaphost.com/sitemap-host3.xml
```

To avoid cross-submission errors you must prove ownership of `www.host1.com` (and others) by adding a `Sitemap:` directive to `http://www.host1.com/robots.txt` that points to the hosted sitemap. Search engines treat the presence of that robots.txt entry as proof that the site owner authorizes the external sitemap.

When a host's `robots.txt` points to a sitemap on another host, all URLs listed in that external sitemap are expected to belong to the host that owns the `robots.txt` pointing to it.

## Validating your Sitemap

Schemas:

- Sitemaps: <http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd>
- Sitemap index: <http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd>

Tools for XML schema validation:

- <http://www.w3.org/XML/Schema#Tools>
- <http://www.xml.com/pub/a/2000/12/13/schematools.html>

To validate against the XSD, include schema headers in the root element.

Sitemap example with schema headers:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<urlset xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"
            xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
   <url>
      ...
   </url>
</urlset>
```

Sitemap index example with schema headers:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<sitemapindex xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd"
            xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
   <sitemap>
      ...
   </sitemap>
</sitemapindex>
```

## Extending the Sitemaps protocol

You can extend the Sitemaps protocol using your own namespace by specifying it in the root element. Example:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<urlset xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"
            xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
            xmlns:example="http://www.example.com/schemas/example_schema"> <!-- namespace extension -->
   <url>
      <example:example_tag>
         ...
      </example:example_tag>
   </url>
</urlset>
```

## Informing search engine crawlers

After creating and publishing your Sitemap, inform supporting search engines by:

1. Submitting it via the search engine's submission interface (refer to each search engine's docs).
2. Adding the Sitemap location to your `robots.txt` file.
3. Sending an HTTP request (ping) to the search engine.

### Specifying Sitemap location in `robots.txt`

Add a line with the full URL to the sitemap, for example:

```
Sitemap: http://www.example.com/sitemap.xml
```

You can list multiple `Sitemap:` lines in a single `robots.txt` file.

### Submitting via HTTP request (ping)

Replace `<searchengine_URL>` with the URL provided by the search engine and URL-encode the sitemap URL after `/ping?sitemap=`.

Example:

```
<searchengine_URL>/ping?sitemap=http%3A%2F%2Fwww.yoursite.com%2Fsitemap.gz
```

You can use `wget`, `curl`, or any HTTP client. A successful request returns HTTP 200 (this indicates receipt, not validity of the sitemap content).

## Excluding content

To exclude content from search engines, use `robots.txt` or `robots` meta tags. See <https://www.robotstxt.org> for details.

---

Last Updated: Monday, November 21, 2016

Terms and conditions
