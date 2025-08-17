
CREATE TABLE posts (id bigint, title text, content text, tags multi64) charset_table='non_cjk,cjk' morphology='stem_en,icu_chinese';

-- Insert statements sample
-- REPLACE INTO posts (id, title, content, tags) VALUES(3, 'helloworld', 'happy happy happy', (1,2));
-- REPLACE INTO posts (id, title, content, tags) VALUES(4, '文章1', '这是一台8口交换机，用于网络数据传输。', (1,2));

-- Query statements sample
-- SELECT *, weight() FROM posts WHERE MATCH('is|happy') OPTION ranker=sph04;

