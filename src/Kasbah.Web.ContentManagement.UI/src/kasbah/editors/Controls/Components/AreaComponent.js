import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form'
import Nested from '../Nested'

export const arrayMove = (array, oldIndex, newIndex) => {
  if (oldIndex === newIndex) {
    return array
  }

  const targetIndex = (newIndex === array.length ? 0 : (newIndex === -1 ? newIndex = array.length : newIndex))
  if (oldIndex < targetIndex) {
    return [
      ...array.filter((_, index) => index <= targetIndex && index !== oldIndex),
      array[oldIndex],
      ...array.filter((_, index) => index > targetIndex && index !== oldIndex)
    ]
  } else {
    return [
      ...array.filter((_, index) => index < targetIndex && index !== oldIndex),
      array[oldIndex],
      ...array.filter((_, index) => index >= targetIndex && index !== oldIndex)
    ]
  }
}

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

  handleMove = (offset) => () => {
    const { ent, area, input: { value, onChange } } = this.props
    onChange({
      ...value,
      [area]: arrayMove(value[area], value[area].indexOf(ent), value[area].indexOf(ent) + offset)
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
        <div className='modal-background' onClick={this.handleToggleModal(false)} />
        <div className='modal-card'>
          <header className='modal-card-head'>
            <span className='modal-card-title'>
              Edit component properties
            </span>
            <button type='button' className='delete' onClick={this.handleToggleModal(false)} />
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
            <button type='button' className='button is-primary' onClick={this.handleToggleModal(false)}>Close</button>
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
            {component && <span className='level-item'>{component.alias}</span>}
          </div>
          <div className='level-right'>
            <button type='button' className='level-item button is-small is-warning' onClick={this.handleRemove}>
              <span className='icon is-small'>
                <i className='fa fa-trash' />
              </span>
              <span>Remove component</span>
            </button>
            <div className='level-item has-addons'>
              <div className='control'>
                <button type='button' className='button is-small is-light' onClick={this.handleMove(-1)}>
                  <span className='icon is-small'>
                    <i className='fa fa-long-arrow-up' />
                  </span>
                  <span>Move up</span>
                </button>
              </div>
              <div className='control'>
                <button type='button' className='button is-small is-light' onClick={this.handleMove(+1)}>
                  <span className='icon is-small'>
                    <i className='fa fa-long-arrow-down' />
                  </span>
                  <span>Move down</span>
                </button>
              </div>
            </div>
            <button type='button' className='level-item button is-small is-primary' onClick={this.handleToggleModal(true)}>
              {component ? `Edit: ${component.alias}` : 'Select control'}
            </button>
          </div>
        </div>
        <hr />
        {this.modal}
      </div>
    )
  }
}

export default AreaComponent
