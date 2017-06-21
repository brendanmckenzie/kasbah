import React from 'react'
import _ from 'lodash'
import Loading from 'components/Loading'
import { makeApiRequest } from 'store/util'
import Nested from './Nested'
import { Field, FieldArray } from 'redux-form'

const AreaComponent = ({ fields }) => {
  console.log(fields.map(f => f))
  return (
    <li className='component'>
      <ul>
        {fields.map((fld, index) => {
          <li key={index}>
            <Field name={fld} component={Nested} />
          </li>
        })}
      </ul>
    </li>
  )
}

AreaComponent.propTypes = {
  fields: React.PropTypes.object
}

class Area extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    area: React.PropTypes.string.isRequired,
    parent: React.PropTypes.string.isRequired,
    components: React.PropTypes.array.isRequired
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
    const { components, area, parent, input: { value } } = this.props

    const nestedOptions = (ent) => components.find(cmp => cmp.alias === ent.Control)

    return (
      <li>
        <strong>{area}</strong>
        <ul>
          {value[area].map((ent, index) => (
            <Field
              key={index}
              name={`${parent}.${area}[${index}].Properties`}
              component={Nested}
              options={nestedOptions(ent).properties} />
          ))}
          <li>
            <button className='button is-small' onClick={this.handleAddComponent}>
              Add component to{' '}<strong>{area}</strong>
            </button>
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

  constructor() {
    super()

    this.state = { loading: true }
  }

  componentWillMount() {
    makeApiRequest({
      url: '/content/components',
      method: 'GET'
    })
      .then(components => {
        this.setState({
          loading: false,
          components
        })
      })
  }

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
    if (this.state.loading) {
      return <Loading />
    }

    const { input: { value, name } } = this.props
    return (
      <div>
        <ul>
          {_.keys(value).map(area => <Area key={area} parent={name} area={area} {...this.state} {...this.props} />)}
          <li>
            <button className='button is-small' onClick={this.handleAddArea}>Add area</button>
          </li>
        </ul>
      </div>
    )
  }
}

export default Components
