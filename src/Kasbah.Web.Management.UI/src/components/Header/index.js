import React from 'react'
import { NavLink } from 'react-router-dom'

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
        <NavLink className='nav-item' to='/'><strong>KASBAH</strong></NavLink>
      </div>

      <div className='nav-right'>
        {sections.map((ent, index) => (
          <NavLink key={index} className='nav-item' activeClassName='is-active' to={ent.link}>{ent.title}</NavLink>
        ))}
        <NavLink className='nav-item' to='/login'>Logout</NavLink>
      </div>
    </div>
  </header>
)

export default Header
