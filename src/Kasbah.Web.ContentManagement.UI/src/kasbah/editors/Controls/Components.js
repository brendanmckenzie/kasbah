import React from 'react'
import _ from 'lodash'

class AreaComponent extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    area: React.PropTypes.string.isRequired,
    component: React.PropTypes.object.isRequired
  }

  handleRemove = (ev) => {
    ev.preventDefault()
    const { area, component, input: { value, onChange } } = this.props

    onChange({
      ...value,
      [area]: value[area].filter(ent => ent !== component)
    })
  }

  render() {
    const { component } = this.props

    return (<li>
      {component.Control}
      <button className='button is-small'>Edit</button>
      <button className='button is-small' onClick={this.handleRemove}>Remove</button>
    </li>)
  }
}

class Area extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    area: React.PropTypes.string.isRequired
  }

  handleAddComponent = (ev) => {
    ev.preventDefault()
    const { area, input: { value, onChange } } = this.props
    const component = prompt('Which component?')

    if (component) {
      onChange({
        ...value,
        [area]: [
          ...value[area],
          {
            'Control': component
          }
        ]
      })
    }
  }

  render() {
    const { area, input: { value } } = this.props
    return (
      <li>
        <strong>{area}</strong>
        <ul>
          {value[area].map((comp, index) => (
            <AreaComponent key={index} component={comp} {...this.props} />
          ))}
          <li>
            <button className='button is-small' onClick={this.handleAddComponent}>Add component to <strong>{area}</strong></button>
          </li>
        </ul>
      </li>
    )
  }
}

class Components extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    className: React.PropTypes.string
  }

  static alias = 'kasbah_web:components'

  handleAddArea = (ev) => {
    ev.preventDefault()
    const { input: { value, onChange } } = this.props
    const area = prompt('Which area?')

    if (area) {
      onChange({
        ...value,
        [area]: []
      })
    }
  }

  render() {
    const { input: { value } } = this.props
    return (
      <div>
        <ul>
          {_.keys(value).map(area => <Area key={area} area={area} {...this.props} />)}
          <li>
            <button className='button is-small' onClick={this.handleAddArea}>Add area</button>
          </li>
        </ul>
        <pre>DEBUG: {JSON.stringify(value)}</pre>
      </div>
    )
  }
}

export default Components
