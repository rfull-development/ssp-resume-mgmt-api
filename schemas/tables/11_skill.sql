-- Table: public.skill

-- DROP TABLE IF EXISTS public.skill;

CREATE TABLE IF NOT EXISTS public.skill
(
    id bigint NOT NULL DEFAULT nextval('seq_skill_id'::regclass),
    item_id bigint NOT NULL,
    row_id integer NOT NULL DEFAULT 1,
    version integer NOT NULL DEFAULT 1,
    skill_id character varying(64) COLLATE pg_catalog."default",
    CONSTRAINT pk_skill PRIMARY KEY (id),
    CONSTRAINT uq_skill_item_id UNIQUE (item_id, row_id),
    CONSTRAINT fk_item_id FOREIGN KEY (item_id)
        REFERENCES public.item (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.skill
    OWNER to postgres;
