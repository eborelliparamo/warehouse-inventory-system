create table if not exists outbox (
  id uuid primary key,
  kind text not null,                
  occurred_at timestamptz not null,
  stream_id uuid not null,
  version bigint not null,
  payload bytea not null,                
  content_type text not null default 'application/x-protobuf',
  created_at timestamptz not null default now(),
  published_at timestamptz null,
  attempts int not null default 0
);

create index if not exists ix_outbox_created on outbox(created_at) where published_at is null;
create unique index if not exists ux_outbox_stream_ver on outbox(stream_id, version);
