import React from 'react'

const demoSections = [
  'Content',
  'SEO',
  'Categorisation'
]

export const SectionSelector = () => (
  <div className='tabs'>
    <ul className='is-left'>
      {demoSections.map((ent, index) =>
        (<li key={index} className={index === 0 ? 'is-active' : null}><a>{ent}</a></li>))}
    </ul>
    <ul className='is-right'>
      <li><a>Patches</a></li>
    </ul>
  </div>
)

export default SectionSelector
