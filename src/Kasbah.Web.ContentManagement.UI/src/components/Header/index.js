import React from 'react'
import { IndexLink, Link } from 'react-router'

const sections = [
  { link: '/content', title: 'Content' },
  { link: '/media', title: 'Media' },
  { link: '/security', title: 'Security' },
  { link: '/system', title: 'System' }
]

export const Header = () => (
  <header className='nav'>
    <div className='container'>
      <div className='nav-left'>
        <IndexLink className='nav-item' to='/'><strong>KASBAH</strong></IndexLink>
      </div>

      <div className='nav-right'>
        {sections.map((ent, index) => (
          <Link key={index} className='nav-item' activeClassName='is-active' to={ent.link}>{ent.title}</Link>
        ))}
        <Link className='nav-item' to='/login'>Logout</Link>
      </div>
    </div>
  </header>
)

export default Header
