-- View: public.usage_alloc_list

-- DROP VIEW public.usage_alloc_list;

CREATE OR REPLACE VIEW public.usage_alloc_list
 AS
 SELECT ua.skill_id,
    u.id,
    u.summary,
    COALESCE(ua.description, u.description) AS description
   FROM usage_alloc ua
     LEFT JOIN usage u ON ua.usage_id = u.id
  ORDER BY ua.skill_id, u.id;

ALTER TABLE public.usage_alloc_list
    OWNER TO postgres;

