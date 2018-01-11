create table node (
  id uuid primary key not null,

  parent_id uuid references node ( id ),
  type varchar(512) not null,
  alias varchar(512) not null,

  display_name text,

  published_version_id integer,

  id_taxonomy uuid[] not null,
  alias_taxonomy varchar(512)[] not null,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null
);

create table node_content (
  id uuid not null references node ( id ),
  version integer not null,
  content jsonb,
  created_at timestamp default now() not null,

  primary key ( id, version )
);

create table "user" (
  id uuid primary key not null,
  username varchar(512) not null,
  password text not null,
  name text,
  email text,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null,
  last_login_at timestamp,
  disabled_at timestamp
);

create table media (
  id uuid primary key not null,
  file_name text,
  content_type text,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null
);

create table profile (
  id uuid primary key not null,

  attributes jsonb,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null
);

create table campaign (
  id uuid primary key not null,
  name varchar(512) not null,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null
);

create table event (
  id uuid primary key not null,
  profile_id uuid not null references profile ( id ),
  campaign_id uuid references campaign ( id ),

  data jsonb,

  created_at timestamp default now() not null
);

create table profile_attribute (
  id uuid primary key not null,
  profile_id uuid not null references profile ( id ),
  alias varchar(512) not null,

  data jsonb,

  created_at timestamp default now() not null
);
