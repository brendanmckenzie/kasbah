import React from 'react'
import PropTypes from 'prop-types'
import _ from 'lodash'
import Loading from 'components/Loading'
import { makeApiRequest } from 'store/util'
import Nested from './Nested'
import { Field } from 'redux-form'

class Area extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    area: PropTypes.string.isRequired,
    parent: PropTypes.string.isRequired,
    components: PropTypes.array.isRequired
  }

  handleAddComponent = () => {
    const { area, input: { value, onChange } } = this.props
    onChange({
      ...value,
      [area]: [
        ...value[area],
        {}
      ]
    })
  }

  render() {
    const { components, area, parent, input: { value } } = this.props

    return (
      <li>
        <strong>{area}</strong>
        <ul>
          {value[area].map((ent, index) => (
            <div key={index}>
              <div className='field'>
                <label>Control</label>
                <div className='control'>
                  <span className='select is-fullwidth'>
                    <Field name={`${parent}.${area}[${index}].Control`} component='select'>
                      <option value={null}>Select</option>
                      {components.map(cmp => (<option key={cmp.alias} value={cmp.alias}>{cmp.alias}</option>))}
                    </Field>
                  </span>
                </div>
              </div>
              {components.find(cmp => cmp.alias === ent.Control) && (<div className='field'>
                <Field
                  name={`${parent}.${area}[${index}].Properties`}
                  component={Nested}
                  options={components.find(cmp => cmp.alias === ent.Control).properties} />
              </div>)}
            </div>
          ))}
          <li>
            <button type='button' className='button is-small' onClick={this.handleAddComponent}>
              {'Add component to '}<strong>{area}</strong>
            </button>
          </li>
        </ul>
      </li>
    )
  }
}

class Components extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    className: PropTypes.string
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
