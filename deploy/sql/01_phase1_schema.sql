create extension if not exists citext;

create table if not exists inventory_item (
  id uuid primary key,
  sku citext not null unique,          
  name text not null,
  total_quantity integer not null default 0,
  created_at timestamptz not null default now()
);

create index if not exists idx_inventory_item_sku on inventory_item(sku);

create table if not exists stock_movement (
  id uuid primary key,
  sku citext not null references inventory_item(sku) on delete cascade,
  delta integer not null,
  occurred_at timestamptz not null default now()
);
create index if not exists idx_stock_movement_sku_time on stock_movement(sku, occurred_at);
