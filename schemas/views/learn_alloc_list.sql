-- View: public.learn_alloc_list

-- DROP VIEW public.learn_alloc_list;

CREATE OR REPLACE VIEW public.learn_alloc_list
 AS
 SELECT ua.skill_id,
    u.id,
    u.summary,
    COALESCE(ua.description, u.description) AS description
   FROM learn_alloc ua
     LEFT JOIN learn u ON ua.learn_id = u.id
  ORDER BY ua.skill_id, u.id;

ALTER TABLE public.learn_alloc_list
    OWNER TO postgres;

