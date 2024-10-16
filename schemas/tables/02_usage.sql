-- Table: public.usage

-- DROP TABLE IF EXISTS public.usage;

CREATE TABLE IF NOT EXISTS public.usage
(
    id bigint NOT NULL DEFAULT nextval('seq_usage_id'::regclass),
    version integer NOT NULL DEFAULT 1,
    summary character varying(255) COLLATE pg_catalog."default" NOT NULL,
    description text COLLATE pg_catalog."default",
    CONSTRAINT pk_usage PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.usage
    OWNER to postgres;
