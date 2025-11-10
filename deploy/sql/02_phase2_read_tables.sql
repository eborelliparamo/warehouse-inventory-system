create table if not exists item_summary (
  sku                 citext primary key,
  name                text not null,
  quantity            integer not null default 0,
  low_stock_threshold integer not null default 10
);
create index if not exists ix_item_summary_quantity on item_summary(quantity);

create table if not exists audit_log (
  id uuid primary key,
  sku citext not null,
  delta integer not null,
  occurred_at timestamptz not null default now()
);
create index if not exists ix_audit_log_time on audit_log(occurred_at desc);