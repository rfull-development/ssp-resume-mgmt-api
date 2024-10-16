-- Table: public.usage_alloc_seg

-- DROP TABLE IF EXISTS public.usage_alloc_seg;

CREATE TABLE IF NOT EXISTS public.usage_alloc_seg
(
    skill_id bigint NOT NULL,
    version integer NOT NULL DEFAULT 1,
    CONSTRAINT uq_usage_alloc_seg_skill_id UNIQUE (skill_id),
    CONSTRAINT fk_skill_id FOREIGN KEY (skill_id)
        REFERENCES public.skill (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.usage_alloc_seg
    OWNER to postgres;
