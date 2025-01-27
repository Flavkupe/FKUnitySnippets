import http.server
import socketserver
import sys
import os

PORT = int(sys.argv[1]) if len(sys.argv) > 1 else 8080

script_dir = os.path.dirname(os.path.abspath(__file__))
build_dir = os.path.join(script_dir, 'build')
os.chdir(build_dir)

Handler = http.server.SimpleHTTPRequestHandler
httpd = socketserver.TCPServer(("", PORT), Handler)

print(f"Serving at port {PORT}")
httpd.serve_forever()