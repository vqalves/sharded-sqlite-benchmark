import http from 'k6/http';

export default function () {
  const url = 'http://localhost:5000/abc';
  
  const result = http.get(url);

  if(result.status !== 200) {
    console.log(result);
  }
}