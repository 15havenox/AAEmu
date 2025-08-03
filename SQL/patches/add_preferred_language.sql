-- Add preferred_language column to characters table
-- This allows each character to have their own language preference

ALTER TABLE `characters` ADD COLUMN `preferred_language` VARCHAR(10) DEFAULT '' COMMENT 'Player preferred language (pt_br, en_us, etc)';

-- Create index for better performance if needed
CREATE INDEX idx_characters_preferred_language ON `characters` (`preferred_language`);

-- Set default language for existing characters (optional)
-- UPDATE `characters` SET `preferred_language` = 'en_us' WHERE `preferred_language` = '';