require('dotenv').config();
const mysql = require('mysql2/promise');
const Benchmark = require('benchmark');

// Kết nối MySQL
const pool = mysql.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    port: process.env.DB_PORT,
    waitForConnections: true,
    connectionLimit: 10,  // Giới hạn số lượng kết nối
    queueLimit: 0         // Không giới hạn số lượng query trong hàng đợi
});
// Câu lệnh MySQL cần benchmark
const query = `
SELECT u.id, u.display_name 
FROM (
    SELECT u.id 
    FROM users u 
    JOIN posts p ON p.owner_user_id = u.id 
    ORDER BY u.id DESC 
    LIMIT 1, 50
) AS temp 
INNER JOIN users u ON u.id = temp.id;
`

// Hàm benchmark với Benchmark.js
async function runBenchmark() {

    const connection = await pool.getConnection();  // Lấy một kết nối từ pool

    const suite = new Benchmark.Suite;

    suite.add('MySQL Query', {
        defer: true, // Để có thể sử dụng promises
        fn: async (deferred) => {
            await connection.query(query);
            deferred.resolve();
        }
    })
        .on('cycle', function (event) {
            console.log(String(event.target));
        })
        .on('complete', function () {
            console.log('Fastest is ' + this.filter('fastest').map('name'));
        })
        .run({ 'async': true });

    await connection.end();
}

runBenchmark().catch(err => console.error("Error during benchmark:", err));
