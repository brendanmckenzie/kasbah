import React from 'react'
import { Field } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

class Nested extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    options: React.PropTypes.object
  }

  constructor() {
    super()

    this.state = {
      showModal: false
    }
  }

  static alias = 'nested'

  get display() {
    const { input: { name } } = this.props
    const type = this.props.options

    return (
      <blockquote>
        <Tabs>
          {_(type.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
            <Tab key={index} title={ent}>
              <div>
                {type.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
                  <div key={fldIndex} className='control'>
                    <label className='label' htmlFor={`${name}_${fld.alias}`}>{fld.displayName}</label>
                    <Field
                      name={`${name}.${fld.alias}`}
                      id={`${name}_${fld.alias}`}
                      component={kasbah.getEditor(fld.editor)}
                      options={fld.options}
                      type={fld.type}
                      className='input' />
                    {fld.helpText && <span className='help'>{fld.helpText}</span>}
                  </div>
                ))}
              </div>
            </Tab>
          ))}
        </Tabs>
      </blockquote>
    )
  }

  render() {
    return (<div className='field-editor__nested'>
      {this.display}
    </div>)
  }
}

export default Nested
