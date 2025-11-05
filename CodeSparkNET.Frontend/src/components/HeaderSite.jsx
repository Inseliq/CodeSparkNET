import { Link } from 'react-router-dom';

export default function HeaderSite() {
  return (
    <div>
      <Link to="/">Главная</Link>
      <Link to="/privacy">Политика</Link>
    </div>
  )
}