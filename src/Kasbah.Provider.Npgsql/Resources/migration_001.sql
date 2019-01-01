create table kasbah.node (
  id uuid primary key not null,

  parent_id uuid references kasbah.node ( id ),
  type varchar(512) not null,
  alias varchar(512) not null,

  display_name text,

  published_version_id integer,

  id_taxonomy uuid[] not null,
  alias_taxonomy varchar(512)[] not null,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null
);

create table kasbah.node_content (
  id uuid not null references kasbah.node ( id ),
  version integer not null,
  content jsonb,
  created_at timestamp default now() not null,

  primary key ( id, version )
);

create table kasbah."user" (
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

create table kasbah.media (
  id uuid primary key not null,
  file_name text,
  content_type text,

  created_at timestamp default now() not null,
  modified_at timestamp default now() not null
);

create table kasbah.session (
  id uuid primary key not null,

  attributes jsonb,

  created_at timestamp default now() not null,
  last_activity_at timestamp default now() not null
);

create table kasbah.session_activity (
  id uuid primary key not null,

  session_id uuid references kasbah.session ( id ),

  type varchar(128) not null,
  attributes jsonb,

  created_at timestamp default now() not null
);
