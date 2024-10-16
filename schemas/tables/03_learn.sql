-- Table: public.learn

-- DROP TABLE IF EXISTS public.learn;

CREATE TABLE IF NOT EXISTS public.learn
(
    id bigint NOT NULL DEFAULT nextval('seq_learn_id'::regclass),
    version integer NOT NULL DEFAULT 1,
    summary character varying(255) COLLATE pg_catalog."default" NOT NULL,
    description text COLLATE pg_catalog."default",
    CONSTRAINT pk_learn PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.learn
    OWNER to postgres;
