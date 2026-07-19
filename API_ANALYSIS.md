# Google Play API Analysis

## 1. URL Parameters
* **Static parameters:** `rpcids`, `source-path`, `f.sid`, `bl`, `hl`, `authuser`, `soc-app`, `soc-platform`, `soc-device`, `_reqid`, `rt`.
* **Dynamic parameters:** Only the country code changes. The other parameters can be safely hardcoded for scraping purposes.
* **Country parameter:** The country is passed via the `gl` parameter in the URL (e.g., `gl=us` or `gl=ua`).

## 2. Request Body
* **Keyword placement:** The keyword is passed deep inside a highly nested JSON array string assigned to the `f.req` key.
* **Content-Type:** `application/x-www-form-urlencoded;charset=UTF-8`
* **Body changes:** When the keyword changes, only the specific string placeholder (e.g., `"poker"`) inside the `f.req` JSON payload changes. The structure remains identical.

## 3. Response Parsing
* **Location of package names:** Package names are embedded directly inside nested arrays along with a numeric flag, formatted like `["com.package.name",7]`.
* **Sequential extraction:** To extract them in the exact order without losing data, do not parse the invalid JSON. Instead, use a Regular Expression (e.g., `\[\\?"([a-zA-Z0-9_.-]+)\\?",\s*\d+\]`) to find all matches sequentially. Add each match to a `HashSet` to remove duplicates while preserving the order of their first appearance.
