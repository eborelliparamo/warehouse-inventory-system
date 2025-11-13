create extension if not exists citext;

create table if not exists event_stream (
  stream_id uuid primary key,
  aggregate_type text not null,
  sku citext not null unique,
  version bigint not null default 0,
  created_at timestamptz not null default now()
);

create table if not exists event (
  event_id uuid primary key,
  stream_id uuid not null references event_stream(stream_id) on delete cascade,
  version bigint not null,
  type text not null,
  data jsonb not null,
  metadata jsonb not null default '{}'::jsonb,
  occurred_at timestamptz not null
);

create unique index if not exists ux_event_stream_version on event(stream_id, version);
create index if not exists ix_event_stream_id on event(stream_id);
