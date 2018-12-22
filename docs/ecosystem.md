# Ecosystem

The Kasbah ecosystem was designed to be minimalist.  Processing is built to be efficient and as such it does not require heavy processing power.

The segregation of content delivery and management processes makes load balancing and zero-downtime-deployments simplistic.

For cost-effective distributions where uptime is not mission critical, hosting the entire Kasbah application - i.e., databases, content delivery and management - is acceptable on a small sized server.  In Amazon Web Services terms, a `t2.medium` server will happily host a website with a small to mid sized load.

In larger, higher traffic instances a dedicated content management server could run on a private network with multiple geographically distributed load balanced content delivery servers.  This setup could also leverege distributed read-only PostgreSQL replicas along with distributed Redis caches.

As indicated above, content delivery servers require read-only access to the content database, sending analytical data back via a Redis queue processed in batch on the content management server.

A future enhancement to the Kasbah ecosystem could see the processing task externalised to alleviate load from the content management server, but as the goal is to simplify infrastructure, this will be a consideration involving a significant amount of research.
