
CREATE TABLE auth_ticket
(
    id           bigserial NOT NULL,
    auth_scheme  varchar   NOT NULL,
    auth_key     varchar   NOT NULL,
    user_id      varchar   NOT NULL,
    login_id     varchar   NOT NULL,
    time_started int8      NOT NULL,
    time_expired int8      NOT NULL,
    ticket_value bytea     NULL,
    CONSTRAINT auth_ticket_pk PRIMARY KEY (id)
);
CREATE INDEX auth_ticket_auth_key_idx ON auth_ticket USING btree (auth_key);

CREATE TABLE "cache"
(
    cache_id     bigserial NOT NULL,
    cache_key    varchar   NOT NULL,
    cache_value  bytea     NOT NULL,
    time_started int8      NOT NULL,
    time_expired int8      NOT NULL,
    CONSTRAINT cache_pk PRIMARY KEY (cache_id)
);
CREATE INDEX cache_cache_key_idx ON cache USING btree (cache_key);

CREATE TABLE internal_data_protection
(
    key_id        bigserial     NOT NULL,
    friendly_name varchar(1000) NULL,
    "xml"         text          NULL
);
CREATE INDEX internal_data_protection_friendly_name_idx ON internal_data_protection USING btree (friendly_name);

CREATE TABLE post_history
(
    post_history_id int8    NOT NULL,
    post_id         int8    NOT NULL,
    author_id       int8    NOT NULL,
    post_type       varchar NOT NULL,
    title           varchar NULL,
    "content"       varchar NULL,
    time_created    int8    NOT NULL,
    CONSTRAINT post_history_pk PRIMARY KEY (post_history_id)
);
CREATE INDEX post_history_post_id_idx ON post_history USING btree (post_id, time_created DESC);

CREATE SEQUENCE post_history_id_seq
    INCREMENT BY 1
    MINVALUE 10000000
    MAXVALUE 9223372036854775807
    START 10000000
    CACHE 1
    NO CYCLE;

CREATE TABLE posts
(
    post_id      int8    NOT NULL,
    author_id    int8    NOT NULL,
    post_type    varchar NOT NULL,
    title        varchar NULL,
    "content"    varchar NULL,
    time_created int8    NOT NULL,
    time_updated int8    NOT NULL,
    CONSTRAINT posts_pk PRIMARY KEY (post_id)
);
CREATE INDEX posts_time_created_idx ON posts USING btree (time_created DESC);

CREATE SEQUENCE post_id_seq
    INCREMENT BY 1
    MINVALUE 10000000
    MAXVALUE 9223372036854775807
    START 10000000
    CACHE 1
    NO CYCLE;

CREATE TABLE "user"
(
    user_id      int8         NOT NULL,
    username     varchar(100) NOT NULL,
    "password"   varchar(500) NOT NULL,
    nickname     varchar(100) NOT NULL,
    email        varchar(200) NOT NULL,
    time_created int8         NOT NULL,
    time_updated int8         NOT NULL,
    CONSTRAINT user_pk PRIMARY KEY (user_id),
    CONSTRAINT user_username_un UNIQUE (username)
);

CREATE SEQUENCE user_id_seq
    INCREMENT BY 1
    MINVALUE 10000000
    MAXVALUE 9223372036854775807
    START 10000000
    CACHE 1
    NO CYCLE;
