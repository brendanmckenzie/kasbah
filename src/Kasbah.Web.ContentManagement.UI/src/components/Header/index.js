import React from 'react'
import { IndexLink, Link } from 'react-router'

const sections = [
  { link: '/content', title: 'Content' },
  { link: '/media', title: 'Media' },
  { link: '/analytics', title: 'Analytics' },
  { link: '/security', title: 'Security' },
  { link: '/system', title: 'System' }
]

export const Header = () => (
  <header className='nav'>
    <div className='container'>
      <ul className='nav-left'>
        <li className='nav-item'><IndexLink to='/'><strong>KASBAH</strong></IndexLink></li>
      </ul>

      <ul className='nav-right'>
        {sections.map((ent, index) => (
          <li key={index} className='nav-item'>
            <Link activeClassName='is-active' to={ent.link}>{ent.title}</Link>
          </li>
        ))}
        <li className='nav-item'><Link to='/login'>Logout</Link></li>
      </ul>
    </div>
  </header>
)

export default Header
