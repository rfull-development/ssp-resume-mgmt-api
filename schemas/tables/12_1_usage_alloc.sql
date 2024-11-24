-- Table: public.usage_alloc

-- DROP TABLE IF EXISTS public.usage_alloc;

CREATE TABLE IF NOT EXISTS public.usage_alloc
(
    skill_id bigint NOT NULL,
    usage_id bigint NOT NULL,
    description text COLLATE pg_catalog."default",
    CONSTRAINT fk_skill_id FOREIGN KEY (skill_id)
        REFERENCES public.skill (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT fk_usage_id FOREIGN KEY (usage_id)
        REFERENCES public.usage (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.usage_alloc
    OWNER to postgres;
