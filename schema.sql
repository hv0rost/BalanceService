//drop database bank ;
//CREATE database bank ;

CREATE TABLE IF NOT EXISTS public.balance
(
    id integer NOT NULL,
    balance double precision NOT NULL,
    CONSTRAINT balance_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.balance
    OWNER to postgres;

CREATE TABLE IF NOT EXISTS public.history
(
    id integer NOT NULL,
    "moneyAmount" double precision NOT NULL,
    description character varying COLLATE pg_catalog."default" NOT NULL,
    "balanceId" integer NOT NULL,
    date timestamp without time zone NOT NULL,
    CONSTRAINT history_pkey PRIMARY KEY (id),
    CONSTRAINT "history_balanceId_fkey" FOREIGN KEY ("balanceId")
        REFERENCES public.balance (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.history
    OWNER to postgres;