import React from 'react'
import PropTypes from 'prop-types'
import Nested from '../Nested'
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
        <div className='level'>
          <div className='level-left'>
            <strong className='subtitle level-item'><span>Components for <code>{area}</code></span></strong>
          </div>
          <div className='level-right'>
            <button type='button' className='level-item button is-small is-warning'>
              <span className='icon is-small'>
                <i className='fa fa-trash' />
              </span>
              <span>Remove area</span>
            </button>
            <button type='button' className='level-item button is-small is-primary' onClick={this.handleAddComponent}>
              <span>Add component</span>
            </button>
          </div>
        </div>

        {value[area].map((ent, index) => {
          const component = components.find(cmp => cmp.alias === ent.Control)

          return (
            <div key={index}>
              <div className='level'>
                <div className='level-left'>
                  <button type='button' className='level-item button is-small is-white'>
                    {component ? component.alias : 'Select control'}
                  </button>
                </div>
                <div className='level-right'>
                  <button type='button' className='level-item button is-small is-warning'>
                    <span className='icon is-small'>
                      <i className='fa fa-trash' />
                    </span>
                    <span>Remove component</span>
                  </button>
                  <button type='button' className='level-item button is-small is-light'>
                    <span className='icon is-small'>
                      <i className='fa fa-long-arrow-up' />
                    </span>
                    <span>Move up</span>
                  </button>
                  <button type='button' className='level-item button is-small is-light'>
                    <span className='icon is-small'>
                      <i className='fa fa-long-arrow-down' />
                    </span>
                    <span>Move down</span>
                  </button>
                </div>
              </div>
              {/*<div className='field is-horizontal'>
              <div className='field-label is-normal'>
                <label className='label'>Control</label>
              </div>
              <div className='field-body'>
                <div className='field'>
                  <div className='control'>
                    <span className='select is-fullwidth'>
                      <Field name={`${parent}.${area}[${index}].Control`} component='select'>
                        <option value={null}>Select</option>
                        {components.map(cmp => (<option key={cmp.alias} value={cmp.alias}>{cmp.alias}</option>))}
                      </Field>
                    </span>
                  </div>
                </div>
              </div>
            </div>*/}
              {components.find(cmp => cmp.alias === ent.Control) && (<div className='field'>
                <Field
                  name={`${parent}.${area}[${index}].Properties`}
                  component={Nested}
                  options={components.find(cmp => cmp.alias === ent.Control).properties} />
              </div>)}
              <hr />
            </div>
          )
        })}
      </li>
    )
  }
}

export default Area
