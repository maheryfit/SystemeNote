-- Crée le trigger s'il n'existe pas, ou le remplace s'il existe déjà.
CREATE OR ALTER TRIGGER dbo.trg_etudiant_planifsemestre_history
-- Le trigger est attaché à la table dbo.etudiant.
ON dbo.etudiant
-- Il s'exécute après (AFTER) une insertion ou une mise à jour.
AFTER INSERT, UPDATE
AS
BEGIN
    -- Évite d'envoyer au client des messages du type "(x rows affected)".
    SET NOCOUNT ON;

    -- 1) Fermer l'ancienne ligne d'historique "active" quand planif_semestre_id change.

    -- Met à jour la table d'historique (alias h).
    UPDATE h
    -- Renseigne la date de fin = date du jour (convertie en type date, sans l'heure).
    SET h.date_fin = CONVERT(date, GETDATE())
    -- Source des lignes à mettre à jour : table historique + inserted/deleted.
    FROM dbo.historique_semestre_etudiant h
    -- Jointure avec inserted : les lignes "nouvelles" (après INSERT/UPDATE).
    INNER JOIN inserted i ON i.id = h.etudiant_id
    -- Jointure avec deleted : les lignes "anciennes" (avant UPDATE). Sur INSERT, deleted est vide.
    LEFT JOIN deleted d ON d.id = i.id
    WHERE
        -- Sur un UPDATE uniquement : d.id non NULL signifie que la ligne existait avant (donc pas un INSERT).
        d.id IS NOT NULL
        -- Insère une fermeture seulement si la valeur planif_semestre_id a changé.
        -- ISNULL(..., -1) sert à comparer correctement même si l'une des valeurs est NULL.
        AND ISNULL(i.planif_semestre_id, -1) <> ISNULL(d.planif_semestre_id, -1)
        -- Ne ferme que l'enregistrement d'historique "actif" (celui dont date_fin est NULL).
        AND h.date_fin IS NULL;

    -- 2) Insérer une nouvelle ligne d'historique pour la nouvelle planification.

    -- Insère dans la table historique.
    INSERT INTO dbo.historique_semestre_etudiant
        -- Liste des colonnes ciblées par l'INSERT.
        (etudiant_id, planif_semetre_id, date_debut, date_fin)
    -- Les valeurs insérées proviennent d'une requête SELECT (set-based, multi-lignes).
    SELECT
        -- Id de l'étudiant concerné.
        i.id,
        -- La nouvelle valeur de planif_semestre_id.
        i.planif_semestre_id,
        -- Date de début = aujourd'hui (sans l'heure).
        CONVERT(date, i.date_admission),
        -- Date de fin = NULL : signifie "période en cours".
        NULL
    -- inserted contient les lignes après l'opération (INSERT ou UPDATE).
    FROM inserted i
    -- deleted contient les lignes avant l'opération (seulement pour UPDATE).
    LEFT JOIN deleted d ON d.id = i.id
    WHERE
        -- Ne rien insérer si planif_semestre_id est NULL (pas de planification).
        i.planif_semestre_id IS NOT NULL
        AND
        (
            -- Cas INSERT : deleted est vide, donc d.id est NULL.
            d.id IS NULL
            -- Cas UPDATE : on insère seulement si la planification a changé.
            OR ISNULL(i.planif_semestre_id, -1) <> ISNULL(d.planif_semestre_id, -1)
        );
END;
-- Termine le batch SQL (utile/nécessaire dans certains outils pour créer un trigger).
GO
--- DELETE FROM note_etudiant;
--- DBCC CHECKIDENT ('note_etudiant', RESEED, 0);
--- DELETE FROM etudiant;
--- DBCC CHECKIDENT ('etudiant', RESEED, 0);
              