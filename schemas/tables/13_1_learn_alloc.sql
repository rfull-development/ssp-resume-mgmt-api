-- Table: public.learn_alloc

-- DROP TABLE IF EXISTS public.learn_alloc;

CREATE TABLE IF NOT EXISTS public.learn_alloc
(
    skill_id bigint NOT NULL,
    learn_id bigint NOT NULL,
    description text COLLATE pg_catalog."default",
    CONSTRAINT fk_skill_id FOREIGN KEY (skill_id)
        REFERENCES public.skill (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT fk_learn_id FOREIGN KEY (learn_id)
        REFERENCES public.learn (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.learn_alloc
    OWNER to postgres;
