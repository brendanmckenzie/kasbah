import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form'
import Nested from '../Nested'

class AreaComponent extends React.Component {
  static propTypes = {
    ent: PropTypes.object.isRequired,
    index: PropTypes.number.isRequired,
    area: PropTypes.string.isRequired,
    parent: PropTypes.string.isRequired,
    input: PropTypes.object.isRequired,
    components: PropTypes.array.isRequired
  }

  constructor() {
    super()

    this.state = { showModal: false }
  }

  handleRemove = () => {
    const { area, input: { value, onChange } } = this.props
    onChange({
      ...value,
      [area]: value[area].filter(ent => ent !== this.props.ent)
    })
  }

  handleToggleModal = (showModal) => () => this.setState({ showModal })

  get modal() {
    if (!this.state.showModal) {
      return null
    }

    const { ent, index, components, area, parent } = this.props
    const component = components.find(cmp => cmp.alias === ent.Control)

    return (
      <div className='modal is-active'>
        <div className='modal-background' onClick={this.handleHideModal(false)} />
        <div className='modal-card'>
          <header className='modal-card-head'>
            <span className='modal-card-title'>
              Edit component properties
            </span>
            <button type='button' className='delete' onClick={this.handleHideModal(false)} />
          </header>
          <section className='modal-card-body'>
            <div className='field is-horizontal'>
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
            </div>
            <div className='field is-horizontal'>
              <div className='field-label is-normal'>
                <label className='label'>Hint</label>
              </div>
              <div className='field-body'>
                <div className='field'>
                  <div className='control'>
                    <Field name={`${parent}.${area}[${index}].Hint`} component='input' className='input' />
                  </div>
                </div>
              </div>
            </div>
            <hr />

            {component && (<div className='field'>
              <Field
                name={`${parent}.${area}[${index}].Properties`}
                component={Nested}
                options={component.properties} />
            </div>)}
          </section>
          <footer className='modal-card-foot'>
            <button type='button' className='button is-primary' onClick={this.handleHideModal(false)}>Close</button>
          </footer>
        </div>
      </div>
    )
  }

  render() {
    const { ent, components } = this.props
    const component = components.find(cmp => cmp.alias === ent.Control)

    return (
      <div>
        <div className='level'>
          <div className='level-left'>
            {ent.Hint && <span className='level-item'>{ent.Hint}</span>}
            <button type='button' className='level-item button is-small is-white' onClick={this.handleToggleModal(true)}>
              {component ? `${component.alias}: Change control` : 'Select control'}
            </button>
          </div>
          <div className='level-right'>
            <button type='button' className='level-item button is-small is-warning' onClick={this.handleRemove}>
              <span className='icon is-small'>
                <i className='fa fa-trash' />
              </span>
              <span>Remove component</span>
            </button>
            <div className='level-item field has-addons'>
              <div className='control'>
                <button type='button' className='button is-small is-light'>
                  <span className='icon is-small'>
                    <i className='fa fa-long-arrow-up' />
                  </span>
                  <span>Move up</span>
                </button>
              </div>
              <div className='control'>
                <button type='button' className='button is-small is-light'>
                  <span className='icon is-small'>
                    <i className='fa fa-long-arrow-down' />
                  </span>
                  <span>Move down</span>
                </button>
              </div>
            </div>
          </div>
        </div>
        <hr />
        {this.modal}
      </div>
    )
  }
}

export default AreaComponent
