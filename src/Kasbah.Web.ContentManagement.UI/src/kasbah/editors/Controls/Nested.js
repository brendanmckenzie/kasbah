import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

const Nested = ({ input: { name }, options }) => (
  <div className='field-editor__nested'>
    <blockquote>
      <Tabs>
        {_(options.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
          <Tab key={index} title={ent}>
            <div>
              {options.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
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
  </div>
)

Nested.propTypes = {
  input: PropTypes.object.isRequired,
  options: PropTypes.object
}

Nested.alias = 'nested'

export default Nested
