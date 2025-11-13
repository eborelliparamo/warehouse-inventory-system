create extension if not exists citext;

create table if not exists audit_event (
  id uuid primary key,
  stream_id uuid not null,
  version bigint not null,
  sku citext not null,
  type text not null,
  delta integer not null,
  occurred_at timestamptz not null
);

-- para consultas de "time machine"
create index if not exists ix_audit_event_sku_time on audit_event (sku, occurred_at);

-- idempotencia del consumer (al menos-una-vez)
create unique index if not exists ux_audit_event_stream_version on audit_event (stream_id, version);
