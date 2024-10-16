-- Table: public.item

-- DROP TABLE IF EXISTS public.item;

CREATE TABLE IF NOT EXISTS public.item
(
    id bigint NOT NULL DEFAULT nextval('seq_item_id'::regclass),
    guid uuid NOT NULL,
    CONSTRAINT pk_item PRIMARY KEY (id),
    CONSTRAINT uq_item_guid UNIQUE (guid)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.item
    OWNER to postgres;
