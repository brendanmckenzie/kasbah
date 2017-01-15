import React from 'react'
import { Field, reduxForm } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

const NestedForm = ({ handleSubmit, type, onClose }) => (
  <div className='modal is-active'>
    <div className='modal-background' onClick={onClose} />
    <div className='modal-card'>
      <header className='modal-card-head'>
        <span className='modal-card-title'>
          {type.displayName}
        </span>
        <button type='button' className='delete' onClick={onClose} />
      </header>
      <section className='modal-card-body'>
        <Tabs>
          {_(type.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
            <Tab key={index} title={ent}>
              <div>
                {type.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
                  <div key={fldIndex} className='control'>
                    <label className='label' htmlFor={fld.alias}>{fld.displayName}</label>
                    <Field
                      name={fld.alias}
                      id={fld.alias}
                      component={kasbah.getEditor(fld.editor)}
                      options={fld.options}
                      className='input' />
                    {fld.helpText && <span className='help'>{fld.helpText}</span>}
                  </div>
                ))}
              </div>
            </Tab>
          ))}
        </Tabs>
      </section>
      <footer className='modal-card-foot'>
        <button type='button' className='button' onClick={onClose}>Cancel</button>
        <button className={'button is-primary'} onClick={handleSubmit}>Update</button>
      </footer>
    </div>
  </div>
)

NestedForm.propTypes = {
  handleSubmit: React.PropTypes.func.isRequired,
  type: React.PropTypes.object.isRequired,
  onClose: React.PropTypes.func.isRequired
}

export default reduxForm({
  form: 'NestedForm'
})(NestedForm)
